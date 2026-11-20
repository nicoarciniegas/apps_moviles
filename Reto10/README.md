# Reto 10 — Puntos de recolección posconsumo (.NET MAUI)

- **Dataset:** Puntos de recolección de aparatos eléctricos y electrónicos en Norte de Santander (`c2fr-ezse`). El dataset sugerido `m2y3-brbe` ya no existe (responde `dataset.missing`).
- **Endpoint:** `https://www.datos.gov.co/resource/c2fr-ezse.json`
- **Campos:** `programa_de_recolecci_n`, `tipo_de_residuos`, `departamento`, `municipio`, `codigo_dane`, `direccion`, `raz_n_social`.
- **Parámetros:** texto libre (`$q`), municipio (`$where`) y cantidad (`$limit`: 10/25/50). La URL generada se muestra en pantalla.
- **JSON:** solo `System.Text.Json`.
- **Ejecutar:** `dotnet build -f net9.0-android` y desplegar en emulador/dispositivo Android.
