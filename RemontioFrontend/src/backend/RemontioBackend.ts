import { Client, type UserDataDTO } from './BackendBase'

export class RemontioBackend extends Client {
  private static readonly TOKEN_KEY = 'remontio_token'
  private static readonly REFRESH_TOKEN_KEY = 'remontio_refresh_token'
  private static readonly USER_DATA_KEY = 'remontio_user_data'
  private static readonly TOKEN_EXPIRY_KEY = 'remontio_token_expiry'

  constructor(baseUrl: string) {
    super(baseUrl, {
      fetch: (url: RequestInfo, init?: RequestInit) => this.fetchWithAuth(url, init),
    })
  }

  private async fetchWithAuth(url: RequestInfo, init?: RequestInit): Promise<Response> {
    let token = this.getStoredToken()

    if (token && this.isTokenExpired()) {
      if (this.canRefreshToken()) {
        // Try to refresh if within 5-minute window
        const refreshed = await this.refreshAuthToken()
        if (refreshed) {
          token = this.getStoredToken()
        }
      }
    }
    if (token) {
      init = init || {}
      init.headers = {
        ...init.headers,
        Authorization: `Bearer ${token}`,
      }
    }

    const response = await fetch(url, init)

    if (response.status === 401 && token && this.canRefreshToken()) {
      const refreshed = await this.refreshAuthToken()
      if (refreshed) {
        // Retry the request with new token
        const newToken = this.getStoredToken()
        if (newToken && init) {
          init.headers = {
            ...init.headers,
            Authorization: `Bearer ${newToken}`,
          }
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
    return localStorage.getItem(RemontioBackend.USER_DATA_KEY)
  }

  public isTokenExpired(): boolean {
    const expiryStr = localStorage.getItem(RemontioBackend.TOKEN_EXPIRY_KEY)
    if (!expiryStr) return true

    const expiry = parseInt(expiryStr)
    return Date.now() >= expiry - 60000
  }

  public canRefreshToken(): boolean {
    const expiryStr = localStorage.getItem(RemontioBackend.TOKEN_EXPIRY_KEY)
    if (!expiryStr) return false

    const expiry = parseInt(expiryStr)
    const now = Date.now()
    const fiveMinutes = 5 * 60 * 1000

    return now > expiry && now < expiry + fiveMinutes
  }

  public isAuthenticated(): boolean {
    const token = this.getStoredToken()
    if (!token) return false

    if (!this.isTokenExpired()) return true
    return this.canRefreshToken()
  }

  public async refreshAuthToken(): Promise<boolean> {
    try {
      const userId = this.getUserId()
      const refreshToken = this.getStoredRefreshToken()

      if (!userId || !refreshToken) {
        this.clearAuth()
        return false
      }

      const response = await this.getRefreshToken(userId, refreshToken)

      if (response) {
        const data = JSON.parse(response)
        this.setAuthTokens(data.token, data.newRefreshToken, data.user)
        return true
      }

      this.clearAuth()
      return false
    } catch (error) {
      console.error('Token refresh failed:', error)
      this.clearAuth()
      return false
    }
  }

  public clearAuth(): void {
    localStorage.removeItem(RemontioBackend.TOKEN_KEY)
    localStorage.removeItem(RemontioBackend.REFRESH_TOKEN_KEY)
    localStorage.removeItem(RemontioBackend.USER_DATA_KEY)
    localStorage.removeItem(RemontioBackend.TOKEN_EXPIRY_KEY)
  }
}
