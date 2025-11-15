# FPS_Resident_Outbreak
# Outbreak Escape

Outbreak Escape es un videojuego FPS de supervivencia desarrollado en Unity donde el jugador debe escapar de una ciudad devastada por un brote zombi atravesando tres escenarios temáticos: hospital, fábrica y cárcel.

Repositorio:  
https://github.com/Jacklb19/FPS_Resident_Outbreak.git

## Características principales

- Shooter en primera persona con énfasis en exploración, combate por oleadas y jefe final.
- Tres mundos funcionales y diferenciados con objetivos claros.
- IA de enemigos con NavMeshAgent y cuatro tipos de zombis: WarZombie, ZombieGirl, RangedZombie y Boss.
- Sistema modular de armas con dos slots (pistola y M16) usando ScriptableObject `WeaponData`.
- HUD completo: vida, munición, puntuación, objetivos y mira dinámica.
- Física con gravedad en todos los mundos usando Rigidbody y CharacterController.
- Sistema de puntuación acumulada y pantalla final con resumen por mundo y total.

## Mundos del juego

### Mundo 1 – Hospital / Ancianato

- Nivel introductorio centrado en exploración en un edificio médico de dos pisos.
- Objetivo: recoger cuatro suministros médicos que habilitan la salida.
- Baja densidad de WarZombies para aprender movimiento, disparo y cambio de armas.

### Mundo 2 – Fábrica vacía

- Complejo industrial abierto con zonas de producción y coberturas.
- Objetivo: sobrevivir a tres oleadas de enemigos con combinaciones crecientes de zombis.
- Introduce ZombieGirl y RangedZombie para forzar decisiones tácticas y gestión de munición.

### Mundo 3 – Cárcel

- Prisión de máxima seguridad con celdas, patios y zonas administrativas.
- Objetivo: activar cuatro generadores, desbloquear al Boss y derrotarlo para escapar.
- Nivel más complejo que mezcla exploración, puzzle ambiental ligero y combate intenso.

## Jugabilidad y controles

- Movimiento:
  - W, A, S, D: desplazamiento.
  - Ratón: rotación de cámara.
  - Espacio: salto.
  - Shift izquierdo: sprint.
- Combate:
  - Click izquierdo: disparar.
  - 1 y 2: cambiar de arma.
  - R: recargar.

## Arquitectura y sistemas

- Siete escenas principales: MainMenu, LoadingScreen, Level_1_Hospital, Level_2_Factory, Level_3_Prison, GameOverScreen y VictoryScreen.
- `GameManager` como Singleton persistente con `DontDestroyOnLoad` para manejar puntuación, nivel actual y estadísticas globales.
- `LevelManager` / `LevelFlow` para controlar objetivos por mundo y transición de escenas.
- IA de enemigos basada en clase `Zombie` con variantes `WarZombie`, `ZombieGirl`, `RangedZombie` y `Boss`.
- `PlayerController`, `PlayerHealth`, `PlayerWeaponController` y `WeaponInventory` para movimiento, vida y sistema de armas.
- `GameUI` para HUD, pantallas de Game Over y victoria.

## Requisitos y ejecución

- Plataforma objetivo: Windows 10 o superior, CPU tipo i5, 4 GB de RAM y GPU compatible con DirectX 11.
- Para jugar desde el ejecutable:
  - Ejecutar el archivo `OutbreakEscape.exe` incluido en la carpeta de build.
- Para abrir el proyecto en Unity:
  - Clonar el repositorio.
  - Abrir la carpeta en Unity.
  - Cargar la escena `MainMenu` y pulsar Play.

## Pruebas y estado del proyecto

- Plan de pruebas funcionales que valida:
  - Flujo completo de los tres mundos.
  - Sistema de armas, daño, muerte y puntuación.
- Versión estable sin errores críticos en consola y build de Windows validado en entorno de clase.

## Trabajo futuro

- Ampliar variedad de armas y enemigos.
- Mejorar la IA del Boss con patrones más complejos.
- Añadir sistema de guardado de progreso y más opciones de configuración.

## Autores

- Jose Luis Burbano Buchelly  
- Sara Maria Ojeda Lopez  

