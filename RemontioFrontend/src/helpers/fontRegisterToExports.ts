import { RobotoBold } from '@/assets/fonts/RobotoBold'
import { RobotoRegular } from '@/assets/fonts/RobotoRegular'
import jsPDF from 'jspdf'

export function registerFonts(doc: jsPDF) {
  // 1. CZYSZCZENIE STRINGA (Kluczowy krok)
  // Usuwamy ewentualny prefiks "data:..." oraz wszystkie spacje i nowe linie
  const cleanRegularFont = RobotoRegular.replace(/^data:.*?;base64,/, '') // Usuwa prefiks (jeśli istnieje)
    .replace(/[\n\r\s]/g, '') // Usuwa spacje i nowe linie

  const cleanBoldFont = RobotoBold.replace(/^data:.*?;base64,/, '') // Usuwa prefiks (jeśli istnieje)
    .replace(/[\n\r\s]/g, '') // Usuwa spacje i nowe linie

  // Weryfikacja (opcjonalnie, dla pewności w konsoli)
  // Sprawdź czy string ma sensowną długość i jest podzielny przez 4 (wymóg Base64)
  if (cleanRegularFont.length % 4 !== 0 && cleanBoldFont.length % 4 !== 0) {
    console.error('Błąd: Długość ciągu Base64 jest nieprawidłowa. Sprawdź plik źródłowy.')
  }

  // 2. Dodanie do VFS
  try {
    doc.addFileToVFS('Roboto-Regular.ttf', cleanRegularFont)
    doc.addFileToVFS('Roboto-Bold.ttf', cleanBoldFont)
    // 3. Rejestracja fontu
    doc.addFont('Roboto-Regular.ttf', 'Roboto', 'normal')
    doc.addFont('Roboto-Bold.ttf', 'Roboto', 'bold')

    console.log('Font zarejestrowany poprawnie')
  } catch (e) {
    console.error('Błąd podczas dodawania fontu do VFS:', e)
  }
}
