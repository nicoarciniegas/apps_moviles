# Taller: Menus and Dialog Boxes — Android Tic-Tac-Toe

## Objetivo

Mejorar el juego de Tic-Tac-Toe para Android mediante un menú de opciones y cuadros de diálogo. El menú debe permitir iniciar una nueva partida, cambiar la dificultad de la IA y salir de la aplicación.

## Opciones del menú

El menú debe tener tres opciones:

1. **New Game** — iniciar una nueva partida.
2. **Difficulty** — seleccionar la dificultad de la IA: Easy, Harder o Expert.
3. **Quit** — salir de la aplicación mediante un diálogo de confirmación.

Android permite hasta seis opciones de menú visibles simultáneamente. En el taller se recomienda definir el menú mediante XML en lugar de crearlo directamente en código.

## 1. Modificación de la lógica del juego

En `TicTacToeGame.java` se debe agregar una enumeración para representar los niveles de dificultad:

```java
public enum DifficultyLevel { Easy, Harder, Expert };
```

También se debe mantener el nivel actual:

```java
private DifficultyLevel mDifficultyLevel = DifficultyLevel.Expert;
```

Y crear getter y setter:

```java
public DifficultyLevel getDifficultyLevel() {
    return mDifficultyLevel;
}

public void setDifficultyLevel(DifficultyLevel difficultyLevel) {
    mDifficultyLevel = difficultyLevel;
}
```

### Comportamiento de la IA

- **Easy:** realiza movimientos aleatorios mediante `getRandomMove()`.
- **Harder:** intenta realizar un movimiento ganador mediante `getWinningMove()`. Si no puede ganar, realiza un movimiento aleatorio.
- **Expert:** intenta ganar; si no puede, intenta bloquear al jugador mediante `getBlockingMove()`; si tampoco puede, realiza un movimiento aleatorio.

La lógica indicada para `getComputerMove()` es:

```java
public int getComputerMove() {
    int move = -1;

    if (mDifficultyLevel == DifficultyLevel.Easy)
        move = getRandomMove();

    else if (mDifficultyLevel == DifficultyLevel.Harder) {
        move = getWinningMove();
        if (move == -1)
            move = getRandomMove();
    }

    else if (mDifficultyLevel == DifficultyLevel.Expert) {
        move = getWinningMove();
        if (move == -1)
            move = getBlockingMove();
        if (move == -1)
            move = getRandomMove();
    }

    return move;
}
```

Las funciones `getRandomMove()`, `getWinningMove()` y `getBlockingMove()` deben implementarse tomando como base el código existente. Si modifican temporalmente el tablero para comprobar movimientos, deben dejarlo en el mismo estado en que estaba antes de la llamada.

## 2. Crear el menú mediante XML

Crear:

```text
res/menu/options_menu.xml
```

El archivo debe contener tres elementos:

```xml
<menu xmlns:android="http://schemas.android.com/apk/res/android">
    <item
        android:id="@+id/new_game"
        android:title="New Game"
        android:icon="@drawable/new_game" />

    <item
        android:id="@+id/ai_difficulty"
        android:title="Difficulty"
        android:icon="@drawable/difficulty_level" />

    <item
        android:id="@+id/quit"
        android:title="Quit"
        android:icon="@drawable/quit_game" />
</menu>
```

## 3. Agregar las imágenes del menú

Se necesitan tres imágenes:

- `new_game.png`
- `difficulty_level.png`
- `quit_game.png`

Deben colocarse en `res/drawable`.

El documento original indica que las imágenes no deben superar los 42 píxeles de alto para evitar que obstruyan el texto del menú.

## 4. Mostrar el menú

La actividad debe cargar el menú desde XML mediante `onCreateOptionsMenu()`:

```java
@Override
public boolean onCreateOptionsMenu(Menu menu) {
    super.onCreateOptionsMenu(menu);

    MenuInflater inflater = getMenuInflater();
    inflater.inflate(R.menu.options_menu, menu);

    return true;
}
```

## 5. Responder a las opciones

Sobrescribir `onOptionsItemSelected()` y comprobar el ID de la opción seleccionada:

```java
@Override
public boolean onOptionsItemSelected(MenuItem item) {
    switch (item.getItemId()) {
        case R.id.new_game:
            startNewGame();
            return true;

        case R.id.ai_difficulty:
            showDialog(DIALOG_DIFFICULTY_ID);
            return true;

        case R.id.quit:
            showDialog(DIALOG_QUIT_ID);
            return true;
    }

    return false;
}
```

Crear los identificadores de los diálogos:

```java
static final int DIALOG_DIFFICULTY_ID = 0;
static final int DIALOG_QUIT_ID = 1;
```

> Nota: el documento original señala que `showDialog()` está deprecated.

## 6. Diálogo de dificultad

Utilizar `AlertDialog.Builder` para crear un diálogo con selección única.

Las opciones deben ser:

- Easy
- Harder
- Expert

El nivel actualmente seleccionado debe aparecer inicialmente marcado.

La estructura indicada es:

```java
builder.setTitle(R.string.difficulty_choose);

final CharSequence[] levels = {
    getResources().getString(R.string.difficulty_easy),
    getResources().getString(R.string.difficulty_harder),
    getResources().getString(R.string.difficulty_expert)
};

builder.setSingleChoiceItems(
    levels,
    selected,
    new DialogInterface.OnClickListener() {
        public void onClick(DialogInterface dialog, int item) {
            dialog.dismiss();

            // Establecer el nivel de dificultad de mGame
            // según el elemento seleccionado.

            Toast.makeText(
                getApplicationContext(),
                levels[item],
                Toast.LENGTH_SHORT
            ).show();
        }
    }
);
```

Se deben completar los dos puntos marcados como `TODO`:

1. Determinar qué opción debe aparecer seleccionada inicialmente.
2. Actualizar el nivel de dificultad de `mGame` según la opción seleccionada.

También deben agregarse las cadenas correspondientes a `strings.xml`.

## 7. Diálogo de confirmación para salir

Al seleccionar **Quit**, mostrar un diálogo de confirmación con las opciones **Yes** y **No**.

La estructura indicada es:

```java
builder.setMessage(R.string.quit_question)
    .setCancelable(false)
    .setPositiveButton(
        R.string.yes,
        new DialogInterface.OnClickListener() {
            public void onClick(DialogInterface dialog, int id) {
                AndroidTicTacToeActivity.this.finish();
            }
        }
    )
    .setNegativeButton(R.string.no, null);
```

Al seleccionar **Yes**, se ejecuta `finish()` y la actividad termina. Al seleccionar **No**, no se realiza ninguna acción.

## 8. Verificación

Ejecutar la aplicación y comprobar que:

- **New Game** inicia correctamente una nueva partida.
- **Difficulty** abre el diálogo y permite seleccionar Easy, Harder o Expert.
- La dificultad seleccionada modifica el comportamiento de la IA.
- **Quit** muestra una confirmación.
- **Yes** termina la actividad.
- **No** cancela la salida.

## 9. Reto adicional 1: icono personalizado

Crear un icono propio llamado:

```text
icon.png
```

Colocarlo en:

```text
res/drawable
```

Y modificar `AndroidManifest.xml`:

```xml
<application
    android:icon="@drawable/icon"
    android:label="@string/app_name">
```

Después, ejecutar la aplicación y verificar el icono desde el listado de aplicaciones.

## 10. Reto adicional 2: diálogo About

Crear una opción adicional que muestre un diálogo **About** identificando al programador.

Crear:

```text
res/layout/about_dialog.xml
```

El diálogo puede incluir componentes e imágenes propias.

Para cargar el layout se utiliza `LayoutInflater`:

```java
AlertDialog.Builder builder = new AlertDialog.Builder(this);

Context context = getApplicationContext();

LayoutInflater inflater =
    (LayoutInflater) context.getSystemService(LAYOUT_INFLATER_SERVICE);

View layout = inflater.inflate(R.layout.about_dialog, null);

builder.setView(layout);
builder.setPositiveButton("OK", null);

Dialog dialog = builder.create();
```

## Conceptos principales del taller

- Menús definidos mediante XML.
- `MenuInflater`.
- `onCreateOptionsMenu()`.
- `onOptionsItemSelected()`.
- `AlertDialog.Builder`.
- Diálogos con selección única mediante `setSingleChoiceItems()`.
- Diálogos de confirmación con botones positivos y negativos.
- `Toast`.
- `LayoutInflater`.
- Recursos `res/menu`, `res/drawable` y `res/layout`.
- Manejo de niveles de dificultad mediante `enum`.
- Integración entre interfaz gráfica y lógica del juego.

## Fuente

Frank McCown, Harding University, *Android Application Programming — Challenge: Menus and Dialog Boxes*.
