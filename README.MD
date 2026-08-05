# GAME DESIGN DOCUMENT: CORE MECHANICS
**Project: Sigil of Will - Single-player Gesture-based and Voice Action RPG**

[![Play on itch.io](https://img.shields.io/badge/Play-itch.io-fa5c5c?logo=itch.io)](https://thanhbinh19.itch.io/sigil-of-will)
[![Watch Trailer](https://img.shields.io/badge/Trailer-YouTube-red?logo=youtube)](https://www.youtube.com/watch?v=smnhPDAU6q0) 

## I. State Loop & Controls
The game is designed around two smoothly transitioning states to accommodate the alternating use of a mouse and hand gestures.

### 1. Exploration Mode
**Activation:** Default state when no enemies are in range.

**Objective:** Movement, pathfinding, environmental interaction.

**Controls:**
- **WASD:** Character movement.
- **Mouse (Delta):** Free 360-degree camera rotation. Left-click/Right-click can be assigned to light interactions (picking up items, opening doors).
- **AI Sensor (MediaPipe):** In Standby mode to save system resources.

### 2. Combat Mode
**Activation:** Automatically triggers when entering a "Combat Zone/Arena" or being detected by a monster (Aggro).

**Objective:** Survive and eliminate targets using magic.

**Controls & Transition:**
- **Auto-Lock Camera:** The camera immediately disables mouse input. Instead, the camera uses an algorithm (e.g., `Quaternion.Slerp` in Unity) to automatically lock the view onto the most dangerous/closest enemy. A Target Indicator circle appears under the monster's feet.
- **Right Hand (Releasing Mouse):** The player raises their right hand to the webcam's recognition area to prepare casting "Sigils" while simultaneously chanting spells (fireball, thunderbolt, windblade).
- **Left Hand (Keyboard):** Remains on WASD to control character movement, dodge/backstep, and jump.

## II. Stamina & Mana System

Stamina and Mana are the core components to balance pacing, forcing players to fight with rhythm instead of blindly spamming skills.

**UI Display:** Two distinct resource bars on the screen. Green represents Mana and Blue represents Stamina.

**Stamina Consumption:**
- **Dodge/Backstep/Sprint (Shift key), Jump (Space key):** Consumes a fixed amount of Stamina. Cannot dodge if Stamina is depleted.
- Casting a spell will consume a certain amount of mana depending on the casted spell.

**Stamina Recovery:**
- Automatically recovers quickly when the character is in the None State (not holding the cast key, hand is resting down or not in the camera view) AND not performing a dodge action.

**Exhaustion Penalty:** If Stamina runs out (drops to 0), the character falls into a state where they cannot cast spells or dodge, can only walk, and becomes an easy target for attacks.

**Mana Recovery:**
- Recovers slower than stamina but is not affected by the character's state (except when casting spells; otherwise, it will recover even while running, jumping, or dodging).

## III. Magic Casting System (Gesture & Voice Casting)
Uses AI combined with physics to define the combat style.

### 1. The Trigger
To begin reading gestures (preventing the AI from misinterpreting accidental arm swings), the player MUST hold down the "E" key with their left hand while their right hand performs the gesture and chants.

Upon releasing the "E" key, the casting action immediately ends/is canceled.

### 2. The Sigil Arsenal
The system uses an LSTM network and MediaPipe to recognize real-time action sequences:

**[SIGIL 1] Fireball:**
- **Gesture:** Charge up energy in the hand -> Push strongly forward.
- **Voice:** Say "fireball"
- **Characteristics:** High damage, slow projectile speed.

**[SIGIL 2] WindBlade:**
- **Gesture:** Swing hand to slash horizontally or vertically decisively.
- **Voice:** Say "windblade"
- **Characteristics:** Slashes a wind blade (melee attack) causing Area of Effect (AoE) damage in a cone shape in front. Extremely effective for sweeping through crowds of small swarming monsters.

**[SIGIL 3] Thunderbolt:**
- **Gesture:** Point hand straight at the screen (towards the monster).
- **Voice:** Say "thunderbolt"
- **Characteristics:** Shoots a lightning bolt that instantly hits the target (Hitscan). Massive damage but consumes a lot of mana.

## IV. Combat Loop & Pacing
A typical battle (especially a Boss Fight) will proceed according to the following loop:

1. **Detection:** The game automatically locks onto the target. The player releases the mouse, preparing their right hand and voice to cast spells.
2. **Observation & Dodging:** The left hand uses WASD + Shift to dodge the Boss's attacks (consuming stamina).
3. **Counterattack (Casting):** Find a Window of Opportunity, hold the "E" key, charge up the right hand, push forward, and chant the spell while holding E.
4. **Recovery (None State):** Release the "E" key to complete the casting, and the skill will be executed by the character.
