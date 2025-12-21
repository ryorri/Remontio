// Plik: generate-font.js
import { readFileSync, writeFileSync } from 'fs'
import { join, dirname } from 'path'
import { fileURLToPath } from 'url'

// 1. Ustalanie ścieżek w ES Module (potrzebne, bo __dirname nie istnieje w ESM)
const __filename = fileURLToPath(import.meta.url)
const __dirname = dirname(__filename)

// Konfiguracja nazw plików
const inputFontName = 'Roboto-Bold.ttf'
const outputFileName = 'RobotoBold.ts' // Nazwa pliku wyjściowego

try {
  // Ścieżka do pliku wejściowego (zakładam, że ttf jest w tym samym folderze co ten skrypt)
  const inputPath = join(__dirname, inputFontName)

  // 2. Wczytanie pliku
  console.log(`Wczytuję: ${inputPath}...`)
  const fontBuffer = readFileSync(inputPath)

  // 3. Konwersja na Base64
  const base64String = fontBuffer.toString('base64')

  // 4. Przygotowanie treści pliku .ts
  const fileContent = `export const RobotoBold = '${base64String}'`

  // Ścieżka wyjściowa (zapisze w tym samym folderze)
  const outputPath = join(__dirname, outputFileName)

  // 5. Zapis pliku
  writeFileSync(outputPath, fileContent)

  console.log('✅ Sukces!')
  console.log(`Utworzono plik: ${outputPath}`)
} catch (err) {
  console.error('❌ Błąd:', err.message)
  if (err.code === 'ENOENT') {
    console.error(`Upewnij się, że plik "${inputFontName}" leży w tym samym folderze co skrypt!`)
  }
}
