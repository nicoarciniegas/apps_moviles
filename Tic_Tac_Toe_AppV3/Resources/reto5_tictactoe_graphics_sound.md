# Reto 5 — Graphics and Sound para Tic-Tac-Toe

## Objetivo

Modificar el juego **Tic-Tac-Toe existente** para reemplazar el tablero construido con botones por un **Custom View** que dibuje el tablero y las fichas X/O, agregar **efectos de sonido** para los movimientos del jugador y del computador, y realizar el **reto adicional de esperar 1 segundo antes del movimiento del computador**.

La implementación debe partir del proyecto Tic-Tac-Toe que ya existe. **No se debe crear un juego nuevo desde cero ni reemplazar innecesariamente la lógica existente del juego.** La lógica de `TicTacToeGame` y las reglas actuales deben reutilizarse.

> Fuente: *Android Application Programming — Challenge: Graphics and Sound*, Frank McCown, Harding University. El documento indica que el tablero anterior usa botones y que el reto consiste en utilizar un `View` personalizado, bitmaps X/O y `MediaPlayer` para los sonidos. 

---

## 1. Reemplazar el tablero de botones por un Custom View

Crear una nueva clase:

```text
BoardView
```

que extienda:

```java
android.view.View
```

Esta clase será responsable de dibujar visualmente el tablero y las fichas.

### Requisitos de `BoardView`

Debe:

- Dibujar un tablero de 3 × 3.
- Dibujar las dos líneas verticales y dos horizontales.
- Usar un objeto `Paint`.
- Definir un ancho constante para las líneas del tablero.
- Obtener dinámicamente el ancho y alto del `View` mediante `getWidth()` y `getHeight()`.
- Dibujar las fichas X y O mediante imágenes/bitmaps.
- Tener acceso al objeto existente `TicTacToeGame`.
- Poder determinar qué casilla fue tocada.

Ejemplo de constante:

```java
public static final int GRID_WIDTH = 6;
```

Y un objeto:

```java
private Paint mPaint;
```

Inicializarlo con:

```java
mPaint = new Paint(Paint.ANTI_ALIAS_FLAG);
```

---

## 2. Imágenes X y O

Agregar dos imágenes para representar:

- X del jugador humano.
- O del computador.

Las imágenes deben:

- Estar en un formato compatible con Android, preferiblemente PNG.
- Tener dimensiones razonables; el documento recomienda aproximadamente 100 × 100 px como máximo.
- Tener nombres válidos para recursos Android:
  - minúsculas
  - números
  - guiones bajos
  - sin espacios

Ejemplo:

```text
x_img.png
o_img.png
```

Ubicación:

```text
res/drawable/
```

En `BoardView`, cargar las imágenes mediante `BitmapFactory.decodeResource()`.

Ejemplo:

```java
mHumanBitmap =
    BitmapFactory.decodeResource(getResources(), R.drawable.x_img);

mComputerBitmap =
    BitmapFactory.decodeResource(getResources(), R.drawable.o_img);
```

---

## 3. Constructores e inicialización

`BoardView` debe tener los constructores necesarios y todos deben llamar a un método `initialize()`.

La estructura esperada es equivalente a:

```java
public BoardView(Context context) {
    super(context);
    initialize();
}

public BoardView(Context context, AttributeSet attrs) {
    super(context, attrs);
    initialize();
}

public BoardView(Context context, AttributeSet attrs, int defStyle) {
    super(context, attrs, defStyle);
    initialize();
}
```

El método `initialize()` debe cargar los bitmaps y configurar el `Paint`.

---

## 4. Dibujar el tablero

Sobrescribir:

```java
@Override
public void onDraw(Canvas canvas)
```

No llamar manualmente a `onDraw()`.

Dentro de `onDraw()`:

1. Obtener el ancho:

```java
int boardWidth = getWidth();
```

2. Obtener el alto:

```java
int boardHeight = getHeight();
```

3. Configurar el `Paint`.

El documento utiliza líneas gruesas de color gris claro:

```java
mPaint.setColor(Color.LTGRAY);
mPaint.setStrokeWidth(GRID_WIDTH);
```

4. Calcular el tamaño de cada celda:

```java
int cellWidth = boardWidth / 3;
int cellHeight = boardHeight / 3;
```

5. Dibujar las dos líneas verticales y las dos horizontales.

El tablero debe dividirse correctamente en nueve casillas.

---

## 5. Dibujar X y O

Después de dibujar la cuadrícula, recorrer las nueve posiciones:

```java
for (int i = 0; i < TicTacToeGame.BOARD_SIZE; i++) {
    int col = i % 3;
    int row = i / 3;

    // calcular left, top, right y bottom

    ...
}
```

Para cada posición:

- Si `TicTacToeGame.getBoardOccupant(i)` corresponde al jugador humano, dibujar la X.
- Si corresponde al computador, dibujar la O.
- Si está vacía, no dibujar ninguna imagen.

Usar:

```java
canvas.drawBitmap(
    bitmap,
    null,
    new Rect(left, top, right, bottom),
    null
);
```

Las coordenadas deben calcularse usando:

- `row`
- `col`
- `cellWidth`
- `cellHeight`
- `GRID_WIDTH`

Las imágenes deben quedar dentro de sus respectivas celdas y no invadir las líneas del tablero.

---

## 6. Conectar `BoardView` con `TicTacToeGame`

Agregar a `BoardView`:

```java
private TicTacToeGame mGame;
```

Y un setter:

```java
public void setGame(TicTacToeGame game) {
    mGame = game;
}
```

La actividad principal debe crear/usar el `TicTacToeGame` existente y entregárselo al `BoardView`.

Ejemplo:

```java
mGame = new TicTacToeGame();

mBoardView = findViewById(R.id.board);

mBoardView.setGame(mGame);
```

No duplicar la lógica del juego dentro de `BoardView`.

`BoardView` debe encargarse principalmente de la representación gráfica.

---

## 7. Modificar el layout

En el layout principal, eliminar el `TableLayout` con los botones que anteriormente representaban las nueve casillas.

Reemplazarlo por el `BoardView`.

Debe existir una vista equivalente a:

```xml
<edu.harding.tictactoe.BoardView
    android:id="@+id/board"
    android:layout_width="300dp"
    android:layout_height="300dp"
    android:layout_marginTop="5dp" />
```

**Adaptar el package/nombre de clase al proyecto actual.**

También revisar otros `TextView` o elementos del layout que dependieran de los botones anteriores.

---

## 8. Redibujar el tablero

Como ya no existen botones que actualizar individualmente, cada cambio en el estado del juego debe provocar un redibujado del `BoardView`.

Cuando se inicia una partida nueva:

```java
mGame.clearBoard();
mBoardView.invalidate();
```

Cuando se realiza un movimiento válido:

```java
if (mGame.setMove(player, location)) {
    mBoardView.invalidate();
    return true;
}
```

El método `invalidate()` hace que Android vuelva a ejecutar `onDraw()`.

---

## 9. Detectar toques sobre el tablero

El `BoardView` debe recibir los eventos táctiles.

Crear un `OnTouchListener` en la actividad existente.

El documento utiliza:

```java
int col =
    (int) event.getX() / mBoardView.getBoardCellWidth();

int row =
    (int) event.getY() / mBoardView.getBoardCellHeight();

int pos = row * 3 + col;
```

Por tanto, `BoardView` debe proporcionar:

```java
public int getBoardCellWidth() {
    return getWidth() / 3;
}

public int getBoardCellHeight() {
    return getHeight() / 3;
}
```

La posición calculada debe enviarse a la lógica existente de `TicTacToeGame`.

---

## 10. Mantener la lógica existente del juego

El proyecto ya tiene lógica para:

- realizar movimientos;
- comprobar movimientos legales;
- comprobar ganador;
- determinar empate;
- realizar el movimiento del computador;
- controlar cuándo termina el juego.

Reutilizar esa lógica.

No implementar una segunda versión de las reglas dentro de `BoardView`.

La función equivalente a:

```java
setMove(char player, int location)
```

debe seguir utilizando:

```java
mGame.setMove(player, location)
```

y posteriormente:

```java
mBoardView.invalidate();
```

---

# 11. Agregar sonidos

Agregar dos efectos de sonido:

1. Sonido cuando el jugador humano realiza una jugada.
2. Sonido cuando el computador realiza una jugada.

Los sonidos deben ser cortos, aproximadamente de 1–2 segundos.

Preferiblemente utilizar `.mp3`, ya que el documento menciona que también pueden utilizarse `.wav`, pero recomienda MP3 debido a problemas que pueden presentarse con WAV.

Los archivos deben tener nombres compatibles con recursos Android:

```text
human_move.mp3
computer_move.mp3
```

Sin espacios y utilizando minúsculas, números y `_`.

---

## 12. Carpeta `res/raw`

Crear:

```text
res/raw/
```

Agregar allí los dos archivos de sonido.

Ejemplo:

```text
res/raw/human_move.mp3
res/raw/computer_move.mp3
```

---

## 13. `MediaPlayer`

Crear dos variables:

```java
MediaPlayer mHumanMediaPlayer;
MediaPlayer mComputerMediaPlayer;
```

Inicializar los reproductores en `onResume()`:

```java
@Override
protected void onResume() {
    super.onResume();

    mHumanMediaPlayer =
        MediaPlayer.create(
            getApplicationContext(),
            R.raw.human_move
        );

    mComputerMediaPlayer =
        MediaPlayer.create(
            getApplicationContext(),
            R.raw.computer_move
        );
}
```

Liberarlos en `onPause()`:

```java
@Override
protected void onPause() {
    super.onPause();

    if (mHumanMediaPlayer != null) {
        mHumanMediaPlayer.release();
        mHumanMediaPlayer = null;
    }

    if (mComputerMediaPlayer != null) {
        mComputerMediaPlayer.release();
        mComputerMediaPlayer = null;
    }
}
```

Es importante liberar los recursos para evitar fugas de recursos.

---

## 14. Reproducir sonidos

Cuando el jugador humano realice un movimiento válido:

```java
mHumanMediaPlayer.start();
```

Cuando el computador realice un movimiento válido:

```java
mComputerMediaPlayer.start();
```

El sonido debe reproducirse únicamente cuando realmente se haya realizado un movimiento válido.

---

# 15. Reto adicional: retrasar el movimiento del computador

Implementar el **Extra Challenge** del documento.

Actualmente, después del movimiento humano, el computador realiza inmediatamente su movimiento.

Modificar este comportamiento para que el computador espere aproximadamente:

```text
1 segundo = 1000 ms
```

antes de realizar su movimiento.

El documento indica explícitamente que **no se debe bloquear el UI thread** durante ese segundo.

Utilizar:

```java
android.os.Handler
```

y:

```java
postDelayed()
```

Ejemplo conceptual:

```java
Handler handler = new Handler();

handler.postDelayed(new Runnable() {
    @Override
    public void run() {
        // movimiento del computador
    }
}, 1000);
```

---

## 16. Controlar el turno

Debido al retraso de 1 segundo, se debe evitar que el jugador pueda realizar otra jugada mientras el computador está pensando.

Agregar una variable de estado para controlar el turno, por ejemplo:

```java
private boolean computerTurn;
```

Comportamiento esperado:

### Turno humano

```text
computerTurn = false
```

El usuario puede tocar una casilla.

### Después de un movimiento humano válido

```text
computerTurn = true
```

Mostrar el mensaje existente de que es el turno de Android/computador y programar el movimiento con `postDelayed()`.

### Después del movimiento del computador

```text
computerTurn = false
```

El usuario vuelve a poder realizar una jugada.

---

# 17. Flujo esperado de la partida

El flujo final debe ser:

```text
Inicio de partida
       ↓
Usuario toca una casilla
       ↓
¿La casilla está disponible?
       ↓
      Sí
       ↓
Se coloca X
       ↓
Se actualiza BoardView
       ↓
Suena efecto del jugador
       ↓
¿Hay ganador/empate?
       ↓
      No
       ↓
Turno del computador
       ↓
Mostrar "Android's turn" / mensaje existente
       ↓
Esperar 1 segundo
       ↓
Computador realiza movimiento
       ↓
Se coloca O
       ↓
Se actualiza BoardView
       ↓
Suena efecto del computador
       ↓
¿Hay ganador/empate?
       ↓
      No
       ↓
Turno del usuario
       ↓
Repetir
```

Si existe un ganador o empate después del movimiento humano, **no programar el movimiento del computador**.

---

# 18. Verificaciones obligatorias

Al finalizar la implementación, comprobar:

### Tablero

- [ ] Se muestra una cuadrícula 3 × 3.
- [ ] Las líneas están correctamente ubicadas.
- [ ] La cuadrícula se adapta al tamaño del `View`.
- [ ] Las X aparecen en las posiciones correctas.
- [ ] Las O aparecen en las posiciones correctas.
- [ ] Las imágenes no se salen de las celdas.

### Interacción

- [ ] El usuario puede tocar cualquiera de las 9 casillas.
- [ ] El cálculo de fila/columna es correcto.
- [ ] No se pueden realizar movimientos ilegales.
- [ ] `BoardView.invalidate()` actualiza correctamente la pantalla.
- [ ] El botón/acción de nueva partida limpia correctamente el tablero.

### Sonido

- [ ] Se reproduce un sonido al mover X.
- [ ] Se reproduce un sonido al mover O.
- [ ] No se reproduce sonido para movimientos inválidos.
- [ ] `MediaPlayer` se libera correctamente en `onPause()`.

### Turnos

- [ ] El computador espera aproximadamente 1 segundo.
- [ ] La interfaz no se congela durante la espera.
- [ ] El usuario no puede jugar durante el turno del computador.
- [ ] El movimiento del computador se cancela/no se programa si el movimiento humano ya terminó la partida.

---

# 19. Restricciones de implementación

1. Partir del **Tic-Tac-Toe existente**.
2. Reutilizar `TicTacToeGame`.
3. No reemplazar innecesariamente la lógica existente.
4. `BoardView` debe encargarse de la representación gráfica.
5. La actividad debe encargarse de coordinar interacción, turnos y estado de la partida.
6. Utilizar `invalidate()` para actualizar el tablero.
7. Utilizar `Handler.postDelayed()` para el retraso del computador.
8. No bloquear el UI thread.
9. Utilizar recursos de `res/drawable` para X/O.
10. Utilizar recursos de `res/raw` para sonidos.
11. Liberar correctamente los `MediaPlayer`.

---

# 20. Criterio de finalización

El reto se considera terminado cuando el Tic-Tac-Toe existente conserva toda su funcionalidad y además:

- el tablero ya no depende de botones individuales;
- un `BoardView` personalizado dibuja la cuadrícula;
- X y O se muestran mediante imágenes;
- el usuario puede seleccionar una casilla mediante touch;
- los movimientos actualizan visualmente el tablero;
- existen sonidos independientes para jugador y computador;
- los recursos de audio se gestionan correctamente;
- el computador espera 1 segundo antes de realizar su movimiento;
- el usuario no puede hacer movimientos durante el turno del computador;
- la lógica original de victoria, empate y movimientos se mantiene funcionando.

## Nota para el agente

Antes de modificar código:

1. Inspeccionar la estructura actual del proyecto.
2. Identificar la Activity principal del Tic-Tac-Toe.
3. Identificar la clase `TicTacToeGame`.
4. Identificar cómo se representa actualmente el tablero.
5. Identificar el método actual que procesa los movimientos.
6. Identificar el mecanismo actual para determinar ganador/empate.
7. Reutilizar esas piezas en lugar de duplicarlas.

Después de identificar la estructura existente, implementar el reto realizando los cambios mínimos necesarios para cumplir todos los puntos anteriores.
