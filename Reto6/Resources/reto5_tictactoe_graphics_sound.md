````markdown
# Reto 6 - Change Orientation & Saving State
> Guía de implementación para un Agente de IA

## Objetivo

Modificar la aplicación **Android TicTacToe** para que:

1. Soporte orientación **Portrait** y **Landscape**.
2. Mantenga el estado del juego al cambiar la orientación.
3. Persista los puntajes entre ejecuciones mediante **SharedPreferences**.
4. Agregue una opción para reiniciar los puntajes.
5. Complete los desafíos adicionales si es posible.

El objetivo es implementar el taller sin alterar la arquitectura existente del proyecto. :contentReference[oaicite:0]{index=0}

---

# Contexto

Actualmente la aplicación:

- Solo funciona correctamente en Portrait.
- Al cambiar la orientación se reinicia el juego.
- Se pierden los puntajes.
- Al cerrar completamente la aplicación toda la información desaparece.

El comportamiento esperado es que:

- La interfaz cambie correctamente entre Portrait y Landscape.
- El tablero continúe exactamente donde estaba.
- Los puntajes permanezcan aunque la aplicación se cierre.
- El usuario pueda reiniciar los puntajes cuando quiera. :contentReference[oaicite:1]{index=1}

---

# Restricciones

No modificar la lógica principal del juego más de lo necesario.

Mantener:

- nombres de clases
- arquitectura existente
- flujo del juego

Solo agregar el código necesario para soportar persistencia y orientación.

---

# Parte 1 - Soporte Landscape

## Objetivo

Crear un layout específico para Landscape.

## Tareas

### 1. Modificar AndroidManifest

Eliminar la barra de título utilizando el tema:

```xml
android:theme="@android:style/Theme.NoTitleBar"
```

### 2. Crear

```
res/
    layout-land/
```

### 3. Copiar

Copiar el layout principal hacia:

```
layout-land/activity_main.xml
```

(o el nombre equivalente del proyecto)

### 4. Modificar el nuevo layout

El nuevo layout debe:

- reducir el BoardView aproximadamente a **270dp x 270dp**
- ubicar el tablero al lado izquierdo
- mover los TextView hacia la derecha
- reorganizar la pantalla para Landscape

Puede utilizar:

- RelativeLayout
- ConstraintLayout
- LinearLayout horizontal

según convenga. :contentReference[oaicite:2]{index=2}

---

# Parte 2 - Mantener el estado del juego

## Objetivo

Cuando el dispositivo cambie de orientación NO debe reiniciarse la partida.

---

## Implementar

### Sobrescribir

```java
onSaveInstanceState(Bundle outState)
```

Guardar:

- estado del tablero
- turno actual
- texto informativo
- si terminó el juego
- jugador inicial
- cualquier otra variable necesaria

Usar Bundle mediante pares clave/valor. :contentReference[oaicite:3]{index=3}

---

## Restaurar información

En

```java
onCreate(Bundle savedInstanceState)
```

Si

```java
savedInstanceState != null
```

restaurar:

- tablero
- turno
- información
- variables necesarias

No iniciar una partida nueva cuando exista estado previo. :contentReference[oaicite:4]{index=4}

---

## Implementar en TicTacToeGame

Crear

```java
char[] getBoardState()
```

Debe devolver el estado completo del tablero.

Crear

```java
setBoardState(char[] board)
```

Debe restaurar completamente el tablero. :contentReference[oaicite:5]{index=5} :contentReference[oaicite:6]{index=6}

---

## Mostrar nuevamente los puntajes

Crear

```java
displayScores()
```

que actualice los TextView de:

- Human
- Android
- Tie

después de restaurar el estado. :contentReference[oaicite:7]{index=7}

---

## Corregir bug del turno

Después de restaurar el juego, verificar que el jugador correcto continúe.

Actualmente el computador realiza un movimiento adicional después del cambio de orientación.

Guardar y restaurar también la variable que controla el turno. :contentReference[oaicite:8]{index=8}

---

# Parte 3 - Persistencia usando SharedPreferences

## Objetivo

Los puntajes deben permanecer incluso después de cerrar completamente la aplicación.

---

## Crear

```java
SharedPreferences mPrefs;
```

Inicializar en

```java
onCreate()
```

mediante

```java
getSharedPreferences(...)
```

:contentReference[oaicite:9]{index=9}

---

## Restaurar puntajes

Leer desde SharedPreferences:

- Human Wins
- Computer Wins
- Ties

cuando inicia la aplicación. :contentReference[oaicite:10]{index=10}

---

## Guardar puntajes

Sobrescribir

```java
onStop()
```

Guardar:

- Human Wins
- Computer Wins
- Ties

utilizando:

```java
SharedPreferences.Editor
```

y

```java
commit()
```

:contentReference[oaicite:11]{index=11}

---

# Parte 4 - Reiniciar puntajes

Modificar

```
res/menu/options_menu.xml
```

Eliminar:

```
Quit
```

Agregar:

```
Reset Scores
```

Cuando el usuario lo seleccione:

- Human Wins = 0
- Computer Wins = 0
- Ties = 0

Actualizar inmediatamente la interfaz mediante

```
displayScores()
```

No utilizar Bundle para almacenar estos puntajes, ya que ahora son persistentes mediante SharedPreferences. :contentReference[oaicite:12]{index=12}

---

# Parte 5 - Desafío Extra (Opcional)

## 1

Guardar también el nivel de dificultad.

Actualmente siempre vuelve a Expert.

Persistir este valor usando SharedPreferences.

Como el nivel es un enum:

- guardar un entero
- restaurar el enum correspondiente

:contentReference[oaicite:13]{index=13}

---

## 2

Corregir el fallo cuando se cambia la orientación justo antes del movimiento del computador.

Actualmente:

- puede lanzar excepción
- puede quedar bloqueado esperando el turno del computador

La solución debe garantizar que:

- el Handler no trabaje sobre una Activity destruida
- al recrearse la Activity, si era turno del computador, este realice su movimiento correctamente

:contentReference[oaicite:14]{index=14}

---

# Criterios de aceptación

## Orientación

- [ ] Funciona correctamente en Portrait.
- [ ] Funciona correctamente en Landscape.
- [ ] El layout se adapta correctamente.

---

## Estado

- [ ] El tablero no se reinicia.
- [ ] El turno continúa correctamente.
- [ ] El mensaje del juego permanece.
- [ ] No aparecen movimientos adicionales.

---

## Persistencia

- [ ] Los puntajes sobreviven al cerrar la aplicación.
- [ ] SharedPreferences funciona correctamente.
- [ ] Reset Scores reinicia los valores.

---

## Calidad

- [ ] No romper funcionalidades existentes.
- [ ] Mantener código limpio.
- [ ] Evitar duplicación.
- [ ] Seguir buenas prácticas Android.

---

# Entregables esperados

El agente de IA debe entregar:

1. Código completo implementado.
2. Explicación breve de cada modificación realizada.
3. Lista de archivos modificados.
4. Justificación de cada cambio.
5. Confirmación de que todos los criterios de aceptación fueron cumplidos.
````
