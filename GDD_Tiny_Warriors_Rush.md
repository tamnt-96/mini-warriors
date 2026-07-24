# GDD: Tiny Warriors Rush 1.0 — Idle TD

**Status:** Prototype GDD, core gameplay + shop  
**Version:** 1.0 — 9/6/2026  
**Genre / Tags:** Strategy, Tower Defense, Single Player, Stylized, Fantasy, Medieval Fantasy  
**Monetization:** Ads + In-App Purchases  
**Scope (v1.0):** Early-game loop, FTUE including lobby, shop chest, deck equip, skill tree

---

## 1. Game Overview

### 1a. Short Description

**Tiny Warriors Rush - Idle TD** is a mobile **idle roguelike tower defense** game in a **tower-vs-tower** format.

Each stage: player has a tower on the left, enemy has a tower on the right. Both sides auto-spawn units onto a single horizontal lane. The player does **not** directly control individual units. Decisions are made through:

- Selecting which warriors are unlocked mid-battle
- Choosing random roguelike buff/upgrade cards
- Building a deck before entering battle

- Opening chests to unlock new warriors
- Equipping new warriors into the deck to expand in-battle options

### 1b. Key Differentiators vs. Reference Games

#### i. Faction Counter System + Unique Faction Behaviors

In reference games, factions differ only in appearance — behaviors are largely identical (e.g., assassins don't actually jump to the backline, tankers don't shield teammates).

**This game fixes that:** Each faction has distinct behavior, creating genuine strategic depth.

> Example: Assassin faction dashes behind enemy front lines to attack back-row units.

#### ii. Warrior Evolution (Evolve) Mechanic

Warriors can level up and visually evolve through multiple stages:

```
Lv.1 → Lv.15 → 2-Star (evolved form)
```

Each evolution increases power and changes the warrior's appearance.

### 1c. Design Positioning

> This is a **"light idle strategy"** game — NOT a "skill-based tactical TD."

Players don't need fast reflexes. The core decision is: **which card to pick when a popup appears.**

Depth comes from:
- Random card drafting
- Pre-battle deck composition
- Unit cooldown / spawn intervals
- Buff stacking
- Randomness driving replayability
- Meta progression (skill tree, gacha)

---

## 2. Gameplay Details

### 2a. How a Battle Works (Game Loop)

```
START STAGE
  → STEP 1: CHOOSE BUFF / WARRIOR
  → STEP 2: AUTO BATTLE
  → STEP 3: LEVEL-UP / CHOOSE BUFF
  → STEP 4: DESTROY ENEMY
  → STEP 5: VICTORY / REWARD
  → STEP 6: REPEAT
  → STEP 7: RESULT
```

**Step-by-step breakdown:**

| Step | Description |
|------|-------------|
| **Start Stage** | Player taps Battle from lobby. From Stage 2 onward, entering costs energy. |
| **Choose Buff / Warrior** | At battle start (and each level-up), 3 options appear. Player picks 1 warrior unlock or 1 buff. |
| **Auto Battle** | Warriors auto-spawn from player's tower, march toward the enemy, and auto-attack. No direct unit control. |
| **Level-up / Choose Buff** | When the progress bar fills, battle pauses. Player picks 1 of 3 buffs (ATK, HP, range, attack speed, spawn interval, or new warrior unlock). |
| **Wave Victory** | Killing all enemies in the current wave clears it. |
| **Victory / Reward** | Destroying the enemy tower ends the battle. Rewards: gold, keys, materials, warrior pieces. |
| **Repeat / Result** | Loop back for the next stage; final result screen shown after completing the run. |

---

### 2b. Core Mechanics

#### i. Warrior & Enemy Spawn System

**Warriors:**
- Each warrior has its own individual cooldown
- Warriors selected via talents appear in the spawn queue and begin their countdown to auto-spawn

**Enemies:**
- Spawned from the enemy castle per game designer config
- Typically appear in waves
- Special waves trigger when the enemy castle reaches specific HP thresholds (see Castle System below)

---

#### ii. EXP Gain & Talent Selection

- Killing enemies grants EXP → player levels up
- Each level-up: player picks **1 talent** from their current deck warriors
- **First pick** of a warrior = unlocks that warrior and adds it to the spawn queue
- Picking the same warrior's talents repeatedly makes it stronger
- Selecting the **final talent** of a warrior triggers its **evolution** into a more powerful form

---

#### iii. Skills System

- Players receive SKILLS by equipping them into the deck (like warriors)
- In battle, the skill appears as a talent option to pick
- After picking, the skill icon appears in the **top-right corner** of the screen for the player to activate manually during combat

---

#### iv. Castle System

Both sides defend their own castle. Whichever castle reaches 0 HP first loses.

| Castle | Behavior |
|--------|----------|
| **Player's Castle** | No special mechanics. Losing all HP = defeat. |
| **Enemy Castle** | Has 2 special HP thresholds: **70%** and **30%**. When each threshold is hit, a large enemy wave surges out as a counterattack. Wave details defined in master config. |

---

#### v. Hero System

- Player owns **1 hero** that fights alongside them
- Hero unlocks when the player clears **Chapter 2**
- Hero **always appears in the spawn queue** — no talent pick required to deploy
- Tap the hero card icon (bottom-left of screen) to send the hero into battle immediately
- A hero that dies in battle can be **revived once** via a rewarded ad or gems

---

#### vi. Battle Speed

| Speed | Availability |
|-------|-------------|
| x1.5 | Free — unlocked after starting Chapter 2 |
| x2 | (Separate unlock, details TBD) |

---

#### vii. Counter System

##### Closed Counter Loop (6 Factions)

Units that counter a faction deal **1.5× damage** to that faction.

**Counter relationships (closed loop):**

```
Magic → counters → Tanker
Tanker → counters → Spearman
Spearman → counters → Cavalry
Cavalry → counters → Ranged
Ranged → counters → Infantry
Infantry → counters → Magic
```

---

### 2c. Warrior Types & Behaviors

| Faction | Role | Stats Profile |
| ------- | ---- | ------------- |
| **Infantry** | Melee DPS, balanced frontliner | Balanced HP & ATK |
| **Tanker** | Melee damage sponge, absorbs hits for the team | Very high HP, low ATK |
| **Ranged** | Primary ranged DPS | Very low HP, high ATK |
| **Cavalry** | Fast melee striker | High ATK, fast move speed, medium HP |
| **Magic** | Ranged AOE DPS | Very low HP, high ATK, AOE skills |
| **Spearman** | Melee DPS, balanced frontliner | Balanced HP & ATK |

---

### 2d. Faction Behaviors (Counter System Detail)

Each faction has inherent behavioral traits:

| Faction | Behavior |
|---------|----------|
| **Infantry** | Melee — balanced damage absorber and attacker |
| **Tanker** | Melee — specializes in absorbing damage; very high HP, low damage output |
| **Ranged** | Ranged — primary long-range damage source; very fragile |
| **Cavalry** | Melee — fast-moving striker; high damage, medium durability |
| **Magic** | Ranged — AOE attacker; high damage via area skills, very fragile |
| **Spearman** | Melee — balanced melee unit similar to Infantry |

---

## 3. Out-of-Battle Systems (Meta Loop)

*(Detailed specs in separate sections of master GDD)*

| System | Summary |
|--------|---------|
| **Deck Building** | Equip up to N warriors + skills into deck before battle |
| **Chest / Gacha** | Open chests with keys to unlock new warrior pieces |
| **Skill Tree** | Persistent upgrade tree unlocked via progression |
| **Shop** | Purchase resources, chests, upgrades |
| **Energy System** | Stages 2+ cost energy to enter |

---

## 4. Monetization Signals

| Type | Usage |
|------|-------|
| **Rewarded Ads** | Hero revival, speed boost (x2?) |
| **In-App Purchase** | Gems, chests, energy refills |

---

*End of GDD v1.0 — Scope: Early-game loop + FTUE*
