# Proyecto Reloj Marcador — HU MARCAS

Este módulo corresponde a la HU ADM14 del proyecto “Reloj Marcador”, el cual forma parte de un sistema web para la gestión de asistencia y puntualidad de funcionarios.
Su objetivo principal es permitir que los usuarios realicen marcas de ingreso y salida de forma controlada, segura y con registro en la bitácora.

------------------------------------------------------------

## Requisitos Previos

Antes de ejecutar el proyecto o módulo de marcas, asegúrate de tener instalado:

Visual Studio 2022 o Visual Studio Code

.NET SDK 8.0 o superior

MySQL

Git

------------------------------------------------------------

## Clonar y Restaurar Dependencias

1. Clonar el repositorio:

   git clone URL_DEL_REPOSITORIO
   cd NOMBRE_DEL_PROYECTO

2. Restaurar dependencias:

   dotnet restore

   Esto descargará automáticamente todos los paquetes requeridos definidos en los archivos .csproj del proyecto.

3. Compilar la solución completa:

   dotnet build

4. Ejecutar un proyecto o módulo específico:

   cd NombreDelModulo
   dotnet run

   El servicio se levantará en la dirección (según la configuración del proyecto):
   https://localhost:PUERTO

------------------------------------------------------------

## Restauración Manual de Paquetes (si es necesario)

Si el comando dotnet restore no instala correctamente algún paquete, puedes hacerlo manualmente:

   cd NombreDelModulo
   dotnet add package NOMBRE_DEL_PAQUETE

(Agrega aquí los paquetes específicos que tu proyecto requiera.)

------------------------------------------------------------

## Comandos Útiles

Acción                          | Comando
--------------------------------|-------------------------------------
Restaurar dependencias          | dotnet restore
Compilar solución               | dotnet build
Ejecutar un servicio            | dotnet run
Limpiar binarios                | dotnet clean
Borrar archivos temporales      | git clean -xdf

------------------------------------------------------------

COMO BUENA PRACTICA

recuerda limpiar el proyecto antes de subirlo

entra a VIEW, busca TERMINAL y si estás en la raíz del proyecto (por ejemplo: C:\Users\Usuario\workspace\PROYECTO) corre el siguiente comando:

dotnet clean

Esto ayuda a mantener el repositorio limpio y libre de archivos innecesarios.

------------------------------------------------------------
