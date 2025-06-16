# 🎨 Pixel Wall-E

Welcome to Pixel Wall-E! A cross-platform desktop application for Windows, macOS, and Linux, built with **Avalonia UI**, that allows you to create pixel art using a custom programming language. Guide our robot, Wall-E, to paint on a digital canvas through commands written in an integrated text editor.


---

## 💡 Requirements

To build and run this project from the source code, you will need:

-   **[.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)** or a later version.
-   Your preferred IDE (Visual Studio 2022, JetBrains Rider, or VS Code with the C# Dev Kit extension).

Dependencies for Avalonia and other required libraries will be automatically downloaded via NuGet when you build the project.

---

## 👾 How to Run the Project

1.  Clone this repository:
    ```bash
    git clone https://github.com/Dimoon2/Pixel-Wall-E
    ```
2.  Navigate to the project folder:
    ```bash
    cd PixelWallEApp
    ```
3.  Run the application using the .NET CLI:
    ```bash
    dotnet run
    ```

Alternatively, you can open the solution file (`.sln`) in your IDE and run it from there.

---

## 🛞 How is the project structured?

The project is divided into three main layers that work together:

### 1. **Core (The Interpreter)**
This is the central logic that brings the Wall-E language to life. It is split into three phases:

-   **`Lexer` (Lexical Analysis):** Scans the source code written in the editor and converts it into a sequence of "tokens" (keywords, numbers, operators, etc.). It detects invalid character errors.
-   **`Parser` (Syntax Analysis):** Takes the list of tokens and organizes it into a tree structure (AST - Abstract Syntax Tree) that represents the hierarchy and meaning of the instructions. It detects syntax errors, like a missing parenthesis or a malformed command.
-   **`Interpreter` (Execution):** Traverses the AST and executes each instruction one by one. This is where the state of the canvas and Wall-E is manipulated, variables and loops are handled, and runtime errors (like a `Spawn` command outside the canvas bounds) are detected.

### 2. **Models (The World State)**
This layer contains the classes that represent the application's state:

-   **`WallEState`:** Stores Wall-E's current position (X, Y), brush color, and size.
-   **`CanvasState`:** Represents the pixel canvas. It holds the color matrix and methods to manipulate it (`SetPixel`, `GetPixel`).
-   **`SymbolTable`:** Manages user-created variables in the code.

### 3. **UI (User Interface with Avalonia)**
The visual layer, built with the Avalonia framework and following the MVVM (Model-View-ViewModel) pattern.

-   **Views:** Define the appearance of the controls (the main window, the canvas control).
-   **ViewModels:** Contain the interface logic. They orchestrate the communication between the Interpreter and the Views.
-   **Controls:**
    -   A **Text Editor** (`AvaloniaEdit`) for writing code.
    -   A custom **Canvas** that renders the `CanvasState`.
    -   An **Output Console** to display logs and errors.
    -   Buttons to **Run**, **Load**, and **Save** `.pw` files.
    -   Fields to **resize** the canvas.

---

## 💻 Execution Flow

1.  The user writes code in the **Text Editor**.
2.  When the **RUN** button is pressed, the `MainWindowViewModel` retrieves the text.
3.  It passes the text to the **Interpreter**, which executes its three phases: `Lexer` -> `Parser` -> `Interpreter`.
4.  During the `Interpreter` phase, instructions (like `DrawLine` or `Fill`) modify the state in the **Models** (`CanvasState` and `WallEState`).
5.  The `Models` notify the **UI** that they have changed.
6.  The canvas **View** is redrawn to reflect the new state, displaying the updated pixel art.

---

## ✨ Available Commands and Functions

The Pixel Wall-E language supports a wide range of operations:

-   **Drawing Commands:** `Spawn`, `Color`, `Size`, `DrawLine`, `DrawCircle`, `DrawRectangle`, `Fill`.
-   **Flow Control:** Labels and conditional jumps with `GoTo [label] (condition)`.
-   **Variables:** Assignment with `variable_name <- expression`.
-   **Expressions:**
    -   Arithmetic: `+`, `-`, `*`, `/`, `%`, `**`.
    -   Boolean: `==`, `>`, `<`, `>=`, `<=`, `&&`, `||`.
-   **Environment Functions:**
    -   `GetActualX()`, `GetActualY()`
    -   `GetCanvasSize()`
    -   `GetColorCount(...)`
    -   `IsBrushColor(...)`, `IsBrushSize(...)`
    -   `IsCanvasColor(...)`

---

## 🗯️ Future Improvements and Extensibility

The code is designed to be easily extensible.

-   **To add a new Command:**
    1.  Create a new class that inherits from `StatementNode` (e.g., `DrawTriangleNode`).
    2.  Implement its logic in the `Execute()` method.
    3.  Add the keyword to the `Lexer` and the parsing rule to the `Parser`. That's it! The interpreter will handle it automatically.

-   **To add a new Function:**
    1.  Add a new entry to the `FunctionHandlers` dictionary.
    2.  Implement the method containing the function's logic.
    3.  Add the keyword to the `Lexer` and the rule to the `Parser`.

-   **Upcoming Ideas:**
    -   **Undo/Redo Buttons:** Implement a "memento" pattern that saves canvas states to allow reversing actions.
    -   **Advanced Syntax Highlighting:** Enhance the text editor's highlighting to be context-aware.
    -   **New Loop Types:** Add native support for `for` or `while` loops to the language.
 
 # 🎨 Pixel Wall-E

¡Bienvenido a Pixel Wall-E! Una aplicación de escritorio para Windows, macOS y Linux, construida con **Avalonia UI**, que te permite crear pixel art utilizando un lenguaje de programación propio. Guía a nuestro robot Wall-E para que pinte sobre un lienzo digital mediante comandos escritos en un editor de texto integrado.


---

## 💡 Requisitos

Para compilar y ejecutar este proyecto desde el código fuente, necesitarás:

-   **[.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)** o una versión superior.
-   El IDE de tu preferencia (Visual Studio 2022, JetBrains Rider, o VS Code con la extensión C# Dev Kit).

Las dependencias de Avalonia y otras librerías necesarias se descargarán automáticamente al compilar el proyecto gracias a NuGet.

---

## 👾 ¿Cómo ejecutar el proyecto?

1.  Clona este repositorio:
    ```bash
    git clone [https://github.com/Dimoon2/Pixel-Wall-E]
    ```
2.  Navega a la carpeta del proyecto:
    ```bash
    cd PixelWallEApp
    ```
3.  Ejecuta la aplicación usando el CLI de .NET:
    ```bash
    dotnet run
    ```


## 🛞 ¿Cómo está estructurado el proyecto?

El proyecto se divide en tres capas principales que trabajan en conjunto:

### 1. **Core (El Intérprete)**
Esta es la lógica central que da vida al lenguaje de Wall-E. Se divide en tres fases:

-   **`Lexer` (Análisis Léxico):** Escanea el código fuente escrito en el editor y lo convierte en una secuencia de "tokens" (palabras clave, números, operadores, etc.). Detecta errores de caracteres no válidos.
-   **`Parser` (Análisis Sintáctico):** Toma la lista de tokens y la organiza en una estructura de árbol (AST - Abstract Syntax Tree) que representa la jerarquía y el significado de las instrucciones. Detecta errores de sintaxis, como un paréntesis que falta o un comando mal formado.
-   **`Interpreter` (Ejecución):** Recorre el árbol AST y ejecuta cada instrucción una por una. Es aquí donde se interactúa con el estado del canvas y de Wall-E, se manejan las variables, los bucles y se detectan los errores en tiempo de ejecución (como un `Spawn` fuera de los límites).

### 2. **Models**
Contiene las clases que representan el estado de la aplicación:

-   **`WallEState`:** Almacena la posición actual de Wall-E (X, Y), el color del pincel y su tamaño.
-   **`CanvasState`:** Representa el lienzo de píxeles. Contiene la matriz de colores y métodos para manipularla (`SetPixel`, `GetPixel`).
-   **`SymbolTable`:** Gestiona las variables creadas por el usuario en el código.

### 3. **UI (Interfaz de Usuario con Avalonia)**
La capa visual, construida con el framework Avalonia y siguiendo el patrón MVVM (Model-View-ViewModel).

-   **Vistas (`Views`):** Definen la apariencia de los controles (la ventana principal, el control del canvas).
-   **ViewModels (`ViewModels`):** Contienen la lógica de la interfaz. Orquestan la comunicación entre el Intérprete y las Vistas.
-   **Controles:**
    -   Un **Editor de Texto** (`AvaloniaEdit`) para escribir el código.
    -   Un **Canvas** personalizado que renderiza el `CanvasState`.
    -   Una **Consola de Salida** para mostrar logs y errores.
    -   Botones para **Ejecutar**, **Cargar** y **Guardar** archivos `.pw`.
    -   Campos para **redimensionar** el canvas.

---

## 💻 Flujo de Ejecución

1.  El usuario escribe código en el **Editor de Texto**.
2.  Al pulsar el botón **RUN**, el `MainWindowViewModel` toma el texto.
3.  Se lo pasa al **Intérprete**, que ejecuta sus tres fases: `Lexer` -> `Parser` -> `Interpreter`.
4.  Durante la fase de `Interpreter`, las instrucciones (como `DrawLine` o `Fill`) modifican el estado en los **Models** (`CanvasState` y `WallEState`).
5.  Los `Models` notifican a la **UI** que han cambiado.
6.  La **Vista** del canvas se redibuja para reflejar el nuevo estado, mostrando el pixel art actualizado.

---

## ✨ Comandos y Funciones Disponibles

El lenguaje de Pixel Wall-E soporta una amplia variedad de operaciones:

-   **Comandos de Dibujo:** `Spawn`, `Color`, `Size`, `DrawLine`, `DrawCircle`, `DrawRectangle`, `Fill`.
-   **Control de Flujo:** Etiquetas y saltos condicionales con `GoTo [etiqueta] (condicion)`.
-   **Variables:** Asignación con `nombre_variable <- expresion`.
-   **Expresiones:**
    -   Aritméticas: `+`, `-`, `*`, `/`, `%`, `**`.
    -   Booleanas: `==`, `>`, `<`, `>=`, `<=`, `&&`, `||`.
-   **Funciones del Entorno:**
    -   `GetActualX()`, `GetActualY()`
    -   `GetCanvasSize()`
    -   `GetColorCount(...)`
    -   `IsBrushColor(...)`, `IsBrushSize(...)`
    -   `IsCanvasColor(...)`

---

## 🗯️ Futuras Mejoras y Extensibilidad

El código está diseñado para ser fácilmente extensible.

-   **Añadir un nuevo Comando:**
    1.  Crea una nueva clase que herede de `StatementNode` (ej. `DrawTriangleNode`).
    2.  Implementa su lógica en el método `Execute()`.
    3.  Añade la palabra clave al `Lexer` y la regla de parsing al `Parser`. ¡Eso es todo! El intérprete lo manejará automáticamente.

-   **Añadir una nueva Función:**
    1.  Añade una nueva entrada al diccionario de `FunctionHandlers`.
    2.  Implementa el método que contiene la lógica de la función.
    3.  Añade la palabra clave al `Lexer` y la regla al `Parser`.

-   **Próximas Ideas:**
    -   **Botones de Deshacer/Rehacer (Undo/Redo):** Implementar un sistema de "memento" que guarde los estados del canvas para poder revertir acciones.
    -   **Resaltado de Sintaxis Avanzado:** Mejorar el resaltado en el editor de texto para que sea consciente del contexto.
    -   **Nuevos Tipos de Bucles:** Añadir soporte nativo para bucles `for` o `while` al lenguaje.
 







