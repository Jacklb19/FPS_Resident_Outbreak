# Outbreak Escape

Outbreak Escape es un FPS de supervivencia desarrollado en Unity donde el jugador atraviesa un hospital, una fábrica y una cárcel infestados de zombis, combinando exploración, combate por oleadas y un jefe final.  
Proyecto académico de nivel universitario diseñado como pieza de portafolio para demostrar dominio de programación en C#, arquitectura limpia en Unity e implementación de IA básica para enemigos.

---

## Highlights del proyecto

- FPS en primera persona con tres niveles completos, objetivos claros y dificultad progresiva.
- IA de enemigos con distintos comportamientos: melee estándar, corredor rápido, atacante a distancia y jefe final.
- Sistema modular de armas con dos slots (pistola y rifle M16), recarga, gestión de munición y pickups físicos.
- HUD reactivo con vida, munición, puntuación, objetivos en pantalla y mira dinámica.
- Física con gravedad y colisiones configuradas en todos los mundos (Rigidbody, CharacterController y objetos interactivos).
- Sistema de progresión y puntuación acumulada entre escenas, con pantalla final de resumen.

---

## Stack tecnológico

- **Motor:** Unity (PC – Windows Standalone, x86_64).
- **Lenguaje:** C#.
- **Sistemas de IA:** NavMesh / NavMeshAgent para navegación de enemigos.
- **Arquitectura:** GameManager persistente, escenas desacopladas, prefabs reutilizables, ScriptableObjects para datos de armas.
- **Control de versión:** Git y GitHub.

---

## Gameplay en 3 niveles

- **Mundo 1 – Hospital / Ancianato**  
  Nivel introductorio con pasillos cerrados y ambiente claustrofóbico centrado en exploración.  
  Objetivo principal: recolectar 4 suministros médicos para desbloquear la salida mientras se aprende movimiento, disparo y cambio de armas.

- **Mundo 2 – Fábrica**  
  Escenario industrial abierto orientado al combate por oleadas con coberturas y espacios amplios.  
  Objetivo principal: sobrevivir a 3 oleadas progresivas introduciendo zombis rápidos y a distancia, obligando a gestionar munición y posicionamiento.

- **Mundo 3 – Cárcel**  
  Prisión de máxima seguridad con diseño laberíntico, generadores y jefe final.  
  Objetivo principal: activar 4 generadores para abrir la salida y derrotar a un Boss con vida y daño aumentados.

---

## Sistemas y arquitectura

- **Gestión de juego y escenas**  
  - `GameManager` como Singleton persistente (`DontDestroyOnLoad`) para nivel actual, puntuación y estadísticas.  
  - Escenas separadas para menú, pantalla de carga, 3 niveles jugables, Game Over y Victory.

- **Jugador y armas**  
  - `PlayerController` basado en `CharacterController` con movimiento FPS, salto, sprint y gravedad personalizada.  
  - `PlayerHealth` para vida, daño y flujo de muerte pausando el juego y mostrando Game Over.  
  - `WeaponInventory`, `PlayerWeaponController` y `WeaponPickup` para inventario de dos armas, intercambio rápido, recarga y pickups físicos.  
  - `WeaponData` (ScriptableObject) para parametrizar daño, cadencia, cargador y tiempo de recarga (pistola y M16).

- **IA y enemigos**  
  - Clase base `Zombie` con estados de detección, persecución y ataque usando `NavMeshAgent`.  
  - Variantes:
    - `WarZombie`: enemigo base cuerpo a cuerpo.  
    - `ZombieGirl`: más rápida y agresiva, con menor cooldown entre ataques.  
    - `RangedZombie`: dispara proyectiles desde la distancia y obliga a usar coberturas.  
    - `Boss`: jefe final con mucha más vida, daño elevado y alcance de detección mayor.  
  - `ZombieHealth` gestiona daño, animaciones de muerte y asignación de puntos según tipo.

- **UI / HUD**  
  - `GameUI` para barra de vida, munición actual y total, iconos de armas, puntuación en tiempo real y objetivos de misión.  
  - Crosshair dinámico que reacciona a disparos e impactos, y pantallas de Game Over y Victory con resumen de puntajes.

- **Física y colisiones**  
  - Uso combinado de `CharacterController` para el jugador y `Rigidbody` + colliders en objetos interactivos (cajas, barriles, sillas, etc.).  
  - Proyectiles de enemigos a distancia con tiempo de vida limitado y lógica de colisión optimizada para evitar fugas de rendimiento.

---

## Rol y foco para reclutadores

- Diseño e implementación de sistemas de gameplay en C# (movimiento FPS, armas, vida, puntuación, objetivos).  
- Implementación y ajuste de IA de enemigos mediante NavMesh y una jerarquía de clases heredadas.  
- Estructuración del proyecto en escenas y prefabs reutilizables, aplicando principios de arquitectura limpia y responsabilidad única.  
- Debugging y optimización de colisiones, proyectiles y flujo de niveles hasta conseguir builds estables sin errores críticos en consola.

---

## Cómo ejecutar el proyecto

- Clonar el repositorio:

git clone https://github.com/Jacklb19/FPS_Resident_Outbreak.git

- Abrir el proyecto con la versión de Unity indicada en el repositorio.  
- Cargar la escena `MainMenu` y pulsar **Play** para iniciar desde el flujo completo.
- 
El ejecutable para Windows (build completo) se distribuye como archivo `.zip` adjunto en la sección de Releases
[Build Final](https://github.com/Jacklb19/FPS_Resident_Outbreak/releases/tag/Build)

---

## Estado del proyecto

- Versión estable, utilizada como entrega final de asignatura universitaria.  
- Tres niveles completos con objetivos, enemigos y flujo de juego funcional.  
- Base preparada para futuras mejoras: nuevos tipos de enemigos, más armas, patrones de jefe avanzados y sistema de guardado.

---
