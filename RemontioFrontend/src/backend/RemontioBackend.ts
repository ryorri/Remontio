import { Client, type UserDataDTO } from './BackendBase'

export class RemontioBackend extends Client {
  private static readonly TOKEN_KEY = 'remontio_token'
  private static readonly REFRESH_TOKEN_KEY = 'remontio_refresh_token'
  private static readonly USER_DATA_KEY = 'remontio_user_data'
  private static readonly TOKEN_EXPIRY_KEY = 'remontio_token_expiry'
  private static readonly REFRESH_BUFFER_MS = 60000 // 1 minuta
  private static readonly MAX_REFRESH_WINDOW_MS = 5 * 60000 // 5 minut

  private isRefreshing = false

  constructor(baseUrl: string) {
    super(baseUrl, {
      fetch: (url: RequestInfo, init?: RequestInit) => this.fetchWithAuth(url, init),
    })
  }

  private async fetchWithAuth(url: RequestInfo, init?: RequestInit): Promise<Response> {
    const token = this.getStoredToken()

    if (token) {
      init = init || {}
      init.headers = {
        ...init.headers,
        Authorization: `Bearer ${token}`,
      }
    }

    const response = await fetch(url, init)

    if (response.status === 401 && !this.isRefreshing && this.canRefreshToken()) {
      const refreshed = await this.refreshAuthToken()
      if (refreshed) {
        const newToken = this.getStoredToken()
        if (newToken) {
          init!.headers = { ...init!.headers, Authorization: `Bearer ${newToken}` }
          return fetch(url, init)
        }
      }
    }

    return response
  }

  public setAuthTokens(
    token: string,
    refreshToken: string,
    user: UserDataDTO,
    expiresInMinutes: number = 10,
  ): void {
    const expiresAt = Date.now() + expiresInMinutes * 60 * 1000

    localStorage.setItem(RemontioBackend.TOKEN_KEY, token)
    localStorage.setItem(RemontioBackend.REFRESH_TOKEN_KEY, refreshToken)
    localStorage.setItem(RemontioBackend.USER_DATA_KEY, JSON.stringify(user))
    localStorage.setItem(RemontioBackend.TOKEN_EXPIRY_KEY, expiresAt.toString())
  }

  public getStoredToken(): string | null {
    return localStorage.getItem(RemontioBackend.TOKEN_KEY)
  }

  public getStoredRefreshToken(): string | null {
    return localStorage.getItem(RemontioBackend.REFRESH_TOKEN_KEY)
  }

  public getUserId(): string | null {
    const userDataStr = localStorage.getItem(RemontioBackend.USER_DATA_KEY)
    if (!userDataStr) return null

    try {
      const userData: UserDataDTO = JSON.parse(userDataStr)
      return userData.id || null
    } catch {
      return null
    }
  }

  private getTokenExpiry(): number | null {
    const expiryStr = localStorage.getItem(RemontioBackend.TOKEN_EXPIRY_KEY)
    return expiryStr ? parseInt(expiryStr) : null
  }

  public isTokenExpired(): boolean {
    const expiry = this.getTokenExpiry()
    if (!expiry) return true

    return Date.now() >= expiry + RemontioBackend.REFRESH_BUFFER_MS
  }

  public canRefreshToken(): boolean {
    const expiry = this.getTokenExpiry()
    if (!expiry) return false

    const now = Date.now()
    const refreshStart = expiry - RemontioBackend.REFRESH_BUFFER_MS
    const refreshEnd = expiry + RemontioBackend.MAX_REFRESH_WINDOW_MS

    return now >= refreshStart && now < refreshEnd
  }

  public isAuthenticated(): boolean {
    const token = this.getStoredToken()
    if (!token) return false

    if (!this.isTokenExpired()) return true
    return this.canRefreshToken()
  }

  public async refreshAuthToken(): Promise<boolean> {
    if (this.isRefreshing) return false

    this.isRefreshing = true

    try {
      const userId = this.getUserId()
      const refreshToken = this.getStoredRefreshToken()

      if (!userId || !refreshToken) {
        this.clearAuth()
        return false
      }

      const response = await this.getRefreshToken(userId, refreshToken)
      const data = JSON.parse(response)

      this.setAuthTokens(data.token, data.newRefreshToken, data.user)
      return true
    } catch (error) {
      console.error('Token refresh failed:', error)
      this.clearAuth()
      return false
    } finally {
      this.isRefreshing = false
    }
  }

  public clearAuth(): void {
    localStorage.removeItem(RemontioBackend.TOKEN_KEY)
    localStorage.removeItem(RemontioBackend.REFRESH_TOKEN_KEY)
    localStorage.removeItem(RemontioBackend.USER_DATA_KEY)
    localStorage.removeItem(RemontioBackend.TOKEN_EXPIRY_KEY)
  }
}
