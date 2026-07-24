# Mini Warriors — GDD Data Summary

Source: Google Sheet "TW master config" (`1cl1k2fHmXcgCLLmA0sL2yjqa_MWu6vS8k6rYspEf5Q8`).
The workbook has 13 tabs (doc referred to them as "12 sheets"; `talent_config` is present but currently empty/unfilled by design). This file summarizes the content of every tab so it can drive implementation.

---

## 1. `warrior_config` — Troop base stats (10 rows)

Columns: `Warrior name, ID, Description, Rarity, Type, Base prod speed (s), Troop size, Range, Attack_type, Normal_atk_size (m), Piercing?, Bullets, Bullet_size, HP, Attack, Crit_rate (%), Crit_dmg (%), Movement speed (m/s), Attack interval, Knock_back (m), Knock_back_res (%), Crit_res (%)`

| Name | ID | Rarity | Type | Prod speed | Range | Attack_type | Bullet | HP | Attack | Move spd | Crit dmg % | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Archer | 110 | Rare | Ranged | 3s | 5.25 | 303 (Range_attack) | 501 | 10 | — | 0.56 | 150 | Fragile in melee, multi-shot potential |
| Warrior | 111 | Rare | Infantry | 2s | 1 | 305 (Melee_thrust_1) | — | 16 | 12 | 0.96 | 150 | Versatile melee |
| Spearman | 112 | Rare | Spearman | 3s | 1 | 305 | 509 | 11 | — | 0.96 | 150 | Extended range |
| Lancer | 113 | Epic | Calvary | 5s | 1 | 305 | — | 8 | — | 1.2 | 150 | Charge specialist |
| Alchemist | 114 | Rare | Magic | 4s | 5.25 | 303 | 506 | 5 | — | 0.56 | 150 | Poison |
| Shieldman | 115 | Rare | Heavy | 4s | 1 | 305 | — | 0 | — | 0.96 | 150 | Crit-immune, blocks (Knock_back_res 100%) |
| Berserker | 116 | Epic | Infantry | 3s | 1 | 305 | — | 24 | — | 0.96 | 150 | Leap + whirlwind |
| Wizzard | 117 | Legendary | Magic | 5s | 5.25 | 303 | 512 | 28 | — | 0.56 | 150 | Fireball/meteor |
| Arbalist | 118 | Legendary | Ranged | 3s | 5.25 | 307 (piercing, 9999 pierce) | 508 | 12 | — | 0.56 | 150 | Piercing bolts |
| Lubu | 119 | Legendary | Calvary | 5s | 1 | 301 | 513 | 10 | — | 1.2 | 150 | Charge + knockback |

Some numeric columns (HP/Attack for some rows) came in blank in the raw sheet cells — treat as "TBD/uses formula elsewhere," don't hardcode 0.

---

## 2. `attack_type` — Attack pattern definitions (7 rows)

| ID | Attack_type | AoE? | Description (VN) |
|---|---|---|---|
| 301 | Melee_slash_1 | No | Frontal half-circle hit, 100% dmg to target at hit-center |
| 302 | Melee_slash_2 | Yes | Same half-circle, AoE version |
| 303 | Range_attack | No | Fires a pre-defined bullet at target; guaranteed hit, cannot be dodged |
| 304 | Range_split_1 | Yes | Fires 3 bullets: 1 main (100%) + 2 side (50%); -50% dmg per pierce, max 3 pierces |
| 305 | Melee_thrust_1 | No | Single-target melee thrust |
| 306 | Melee_thrust_2 | Yes | Multi-target melee thrust along a line |
| 307 | Range_piercing_1 | No | Fires 1 bullet piercing X enemies; -50% dmg per pierce, max 3 |

Same sheet also embeds a **Skill** side-table (IDs 401+, e.g. 401 = Leap: "jump into enemy, immune to CC & damage while airborne") — appears to be leftover/legacy columns duplicating part of `troop_skills`.

---

## 3. `bullets` — Bullet/projectile catalog (16 rows)

ID range 501–516, mostly just `ID, Name` (VFX/art column empty — assets pending): Arrow_1..4, Range_split_1, Melee_slash_1..5, Toxic_bomb, Fire_ball, Arbalist Arrow_4, Thrust_1/2, Spear_1, Mage_attack_1.

---

## 4. `enemies_config` — Enemy catalog (13 enemies + type legend)

Columns: `ID, Name, Type, Type_id, Attack_type, Attack_type_id, Bullet, Bullet size, Boss?, HP, Piercing?, Attack, Knock_back, Knock_back_res%, Crit_rate%, Crit_dmg%, Attack interval, Range, Movement speed, Skill, Level, HP multiplier, Attack multiplier`

| ID | Name | Type | HP | Attack | Boss? | Level | Notes |
|---|---|---|---|---|---|---|---|
| 201 | goblin_melee | Infantry | 64 | 22 | No | 1 | |
| 202 | goblin_archer | Ranged | 26 | 10 | No | 2 | |
| 203 | goblin_boss | Infantry | 264 | 45 | **Yes** | 3 | 100% crit_dmg |
| 204 | goblin_calvary | Calvary | 51 | 19 | No | 4 | |
| 205 | goblin_tanker | Tanker | 120 | 28 | No | 5 | move spd 0 |
| 206 | goblin_berserker | Infantry | 100 | 12 | No | 6 | 714s atk interval (typo/outlier?) |
| 207 | goblin_mage | Magic | 50 | 33 | No | 7 | |
| 208 | Sand_infantry | Infantry | 85 | 30 | No | 8 | |
| 209 | Sand_archer_2 | Ranged | 50 | 21 | No | 9 | piercing 9999 |
| 213 | Sand_archer_1 | Ranged | 55 | 25 | No | 10 | |
| 210 | Sand_calvary | Calvary | 110 | 20 | No | 11 | |
| 211 | sand_calvary_boss | Calvary | 1050 | 78 | **Yes** | 12 | |
| 212 | sand_archer_boss | Ranged | 600 | 49 | **Yes** | 13 | piercing 9999 |

Type legend: `1=Infantry, 2=Ranged, 3=Calvary, 4=Magic, 5=Tanker, 6=Spearman`.
Per-level `HP multiplier` / `Attack multiplier` columns exist for scaling by encounter level (mostly ~1.05 per enemy "level" tier).

---

## 5. `troop_skills` — Skill definitions (30 skills, IDs 701–730)

Format: `ID, Troop, Description (with {0}/{1}/{2} placeholders), Skill range, Size, Piercing, X, Y, Z value, Overwrite, Cooldown, FX`.

Per-troop skill kit (2 unique skills each unless noted), plus 8 shared **General** skills (723–730) used as universal stat-boost talents:

- **Warrior** (701, 702): every X attacks, Y% chance of Zx damage strike → evolves into guaranteed heavy strike.
- **Archer** (703, 704): chance to fire extra arrows; periodic multi-target volley with diminishing repeat-hit damage.
- **Spearman** (705, 706, 707): piercing spear thrown at farthest target, upgrades to always-crit + faster recast.
- **Lancer** (708, 709): charge-in knockback on spawn + passive team damage%; post-charge double thrust.
- **Alchemist** (710, 711): poison DoT with death-spread; toxic bomb w/ explosion chance.
- **Shieldman** (712, 713): knockback resist + reduced AoE damage; low-HP damage reduction + stop-attack.
- **Berserker** (714, 715, 716): whirlwind chance; leap-and-invincible on enemy entering range; whirlwind hit-count upgrade.
- **Wizzard** (717, 718): fireball AoE radius; periodic meteor shower.
- **Arbalist** (719, 720): chance for extra arrow; periodic guaranteed-crit extra arrow.
- **Lubu** (721, 722): charging AoE knockback; post-charge guaranteed-crit sweep.
- **General** (723–730): +Attack%, +Crit rate%, +Range%, +Crit dmg%, +Move speed%, +Attack% (dup), +Attack speed%, -Damage taken% — these look like the pool for `talent_config` (currently empty) or generic troop-upgrade passive unlocks referenced in `troop_upgrading`.

Note in-sheet (VN): stacking rule for skill upgrades — subsequent same-skill upgrades multiply off the *current* value, not the base (e.g. +10% HP twice → ×1.1×1.2, not ×1.2 flat), which differs from the normal per-level upgrade stacking model.

---

## 6. `troop_upgrading` — Per-troop leveling table (402 rows = 10 troops × 40 levels)

Columns: `ID, Name, level, Max HP increase, Attack increase, Skill unlock, Passive unlock` + a parallel `level up requirement` block: `level, Gold require, Fragment, Form unlock`.

All 10 troops (Archer, Warrior, Spearman, Lancer, Alchemist, Shieldman, Berserker, Witch, Arbalist, Lubu) have **40 levels**. Pattern (from Archer sample):
- Flat +2 Max HP / +1 Attack per level from level 2 onward.
- Gold cost rises roughly linearly (200 → 3800+ by level 29, continuing up).
- Fragment cost increases in steps (2 → 4 → 6 → 8 → 10 → 12...) roughly every 5 levels.
- Skill unlocks are sparse and tied to specific levels (e.g. Archer: skill 703 unlocks at level 10, skill 704 at level 25).
- Passive unlocks (referencing the General 723–730 skill pool) appear at other specific levels (e.g. passive 723 at levels 5 and 15 for Archer).
- `-1` in Skill/Passive unlock columns = no unlock that level.

This is the primary data source for a **Troop Upgrade system**: cost table + stat growth + skill/passive gating per level, per troop.

---

## 7. `Item` — Currency & material catalog (22 items, IDs 801–822)

| ID | Rarity | Name |
|---|---|---|
| 801 | Common | silver |
| 802 | Common | gold |
| 803 | Common | gems |
| 804 | Rare | rare_fragment (generic) |
| 805 | Epic | epic_fragment (generic) |
| 806 | Legend | legend_fragment (generic) |
| 807 | Common | energy |
| 808 | Common | exp |
| 809 | Common | mission_point |
| 810–819 | Rare/Epic/Legend | per-troop fragments (Archer, Warrior, Spearman, Lancer, Alchemist, Shieldman, Barbarian, Mage, Arbalist, Lubu) |
| 820–822 | Rare/Epic/Legend | random fragment (troop) pools |

---

## 8. `castle_config` — Player base defense (single row)

`Castle HP = 2000`, `Canon Damage = 60`, `Range = 5m`, `Bullet AoE range = 3m`. Single global config, not leveled here (leveling appears to hook into `chapter_config`'s Fortress table and `task_config`'s "upgrade castle HP/level" tasks).

---

## 9. `player_level` — Player XP curve (200 levels)

Columns: `ID, Level, Exp, NeedExp, Gold, Gem` reward per level. Exp curve ramps gradually (30 → 35 → 40 ... accelerating gaps, e.g. +90 at lvl 11, +110 at lvl 12). Reward is flat `500 gold / 50 gem` per level-up in the sampled rows (need to confirm whether this stays flat for all 200 rows or changes later — sampled only first 20).

---

## 10. `wave_config` — Stage/level rewards (15 stages)

Columns: `StageDataList/ID (601-615), index, Name, AtkScale, HpScale, Gold, Exp, Gem, item`. AtkScale/HpScale = 1 for all 15 (no scaling applied yet at this tier — scaling appears to be handled instead via `enemies_config`'s per-enemy Level multiplier and `chapter_config`'s HpScale/AtkScale/CritScale columns). Gold reward ramps 100→1000, Exp 30→80, Gem flat 20. Stage 603 grants item 813 (Lancer epic fragment) as a one-off reward.

---

## 11. `chapter_config` — Wave spawn table + fortress leveling + level-chest rewards (sparse, WIP)

Three sub-tables packed into one sheet (only Chapter 1 filled in as an example):

**a) Enemy spawn per wave** (`Enemy show, Enemy_Show_Name, ID, Chapter, Wave, Group, Quantity, Creattime, EnemyLv, Interval, Fortress HP, HpScale, AtkScale, CritScale, wave_exp`):
- Chapter 1, Wave 1: 2× goblin_melee (Fortress HP 2400, interval 1s).
- Chapter 1, Wave 2: 3× goblin_melee.
- Chapter 1, Wave 3: 5× goblin_melee.
(Only 3 waves populated — rest of chapter/wave grid is empty, presumably TBD.)

**b) Fortress level table** (`Level, Exp_require, Level, each`): a small per-fortress-level exp curve (only 2 rows filled: Lv1 needs 100 exp, Lv2 needs 150).

**c) Level chest rewards** (`Chapter_id, Wave, reward_id, subId, quantity, Note`): exp-per-level curve going up to level 25 (100→1050 stepping ~50 each level), mostly empty in the reward columns — looks like placeholder scaffolding, not yet designed.

**Conclusion:** this sheet is the least complete — treat as a schema/template to implement against, not final content.

---

## 12. `talent_config` — Empty

No data at all. Likely intended to hold a talent tree built from the "General" skill pool (723–730 in `troop_skills`), but not yet authored by the designer. Flag this to the user before building a Talent system off it — nothing here to drive implementation.

---

## 13. `task_config` — Tasks / Achievements / Weekly / Accumulated-chest milestones (560 rows)

Columns: `Features, Task, Target {0}, Description, ID, Reward(type, amount), mission point` + a separate **Acc chest** milestone block (`Milestone, MP required, Reward type/ID/amount`) + `Note`.

Four `Features` groups:

1. **Check-in** — 7-day login reward ladder (gold → legend material → castlestone → legend material → gear ticket → legend material → legend troop card, alt rewards gem/mythic material), plus a `loop` continuation entry after day 7. Each check-in day also feeds a shared "Acc chest" milestone track (troop fragments at increasing MP thresholds, escalating through legendary-tier troops).
2. **Daily task** — ~13 repeatable objectives (log in, upgrade troop, win normal level, purchase energy, merge gear tiers 3/4, gacha troop/gear, watch ad, place gear, purchase IAP, upgrade castle HP, win elite/nightmare level). Each awards mission points (10–30 MP) feeding a daily Acc-chest milestone ladder (coin → gear ticket → energy → rare material → gem, 20→100 MP thresholds).
3. **Weekly** — same task set as Daily but with bigger targets (e.g. log in 5×, win normal level 20×) and its own Acc-chest ladder (gear ticket → legend material → gold → mythic material → gem, 20→100 MP).
4. **Achievement** — one-time milestone chains per task type (e.g. "log in" at 3/7/10/14/21/28/35/42/49/56/63/70 times) with escalating gem rewards (8→75+).

Task target/description use a `{0}` placeholder pattern for numeric goals (e.g. "Log in {0} time(s)") — the same string-templating convention used in `troop_skills` descriptions.

Numeric task IDs (11007–11037 range) are shared across Daily/Weekly/Achievement — same task definition reused with different targets/rewards per feature tier.

---

## Cross-sheet ID ranges (for reference)

| Range | Meaning |
|---|---|
| 110–119 | Troop IDs |
| 201–213 | Enemy IDs |
| 301–307 | Attack type IDs |
| 401+ | (legacy) Skill IDs embedded in attack_type sheet |
| 501–516 | Bullet IDs |
| 601–615 | Stage/wave IDs |
| 701–730 | Troop skill IDs (701–722 per-troop, 723–730 general/talent pool) |
| 801–822 | Item/currency/fragment IDs |
| 11007–11037 | Task definition IDs |

## Gaps / things to confirm with the designer before implementing

- `talent_config` is empty — no talent tree data exists yet.
- `chapter_config` only has Chapter 1 / Waves 1–3 populated; the rest of the spawn grid, fortress level table, and level-chest rewards are stubs.
- Several `warrior_config` numeric cells (HP/Attack for some troops) were blank in the source — needs designer follow-up, don't assume 0.
- `enemies_config` goblin_berserker's attack interval (714s) looks like a data-entry outlier — worth flagging.
- `player_level` reward flatness (500 gold/50 gem) was only confirmed for levels 1–20 of 200; should re-check full column before hardcoding a flat reward.
