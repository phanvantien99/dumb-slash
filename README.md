# Hack & Slash Combo System — Unity

## Architecture Overview

```
┌──────────────────────────────────────────────────────────────────┐
│                    DATA LAYER (ScriptableObjects)                │
│   CharacterComboProfile  ──►  ComboData  ──►  ComboBranch        │
│                                               └──► AttackStep   │
└──────────────────────────────────────────────────────────────────┘
                              │ loaded by
┌──────────────────────────────────────────────────────────────────┐
│                    CORE LAYER                                    │
│                                                                  │
│  PlayerInputHandler  ──►  ComboController  ◄──  CancelSystem     │
│                               │    │                            │
│                          InputBuffer  HitboxController           │
│                               │                                 │
│                          IComboEvents (events)                  │
└───────────────────────────┬──────────────────────────────────────┘
                            │ events consumed by
┌───────────────────────────▼──────────────────────────────────────┐
│                    PRESENTATION / GAME LAYER                     │
│   CombatMediator (damage, VFX, SFX)                             │
│   ComboUIDisplay (HUD counter)                                  │
│   (optional) per-character script for unique reactive behavior  │
└──────────────────────────────────────────────────────────────────┘
```

## File Structure

```
ComboSystem/
├── Core/
│   ├── ComboController.cs      ← Main state machine
│   ├── IComboEvents.cs         ← Event interface
│   ├── InputBuffer.cs          ← Buffered input (recovery window)
│   ├── CancelSystem.cs         ← Cancel rules
│   ├── HitboxController.cs     ← Physics overlap + multi-hit guard
│   ├── PlayerInputHandler.cs   ← New Input System bridge
│   ├── DamageReceiver.cs       ← Enemy health component
│   └── CombatMediator.cs       ← Routes hits → damage/VFX/SFX
├── Data/
│   ├── ComboData.cs            ← ScriptableObject: combo tree
│   └── CharacterComboProfile.cs← ScriptableObject: per-character
└── UI/
    └── ComboUIDisplay.cs       ← HUD combo counter
```

## Quick Setup

### 1. Create a Combo (ScriptableObject)

```
Assets → Right-click → HackAndSlash → Combo Data
```

- Set `root.step.requiredInput = Light` for the first hit
- Add `branches` for follow-up inputs (L → L → H for a finisher, etc.)

### 2. Create a Character Profile

```
Assets → Right-click → HackAndSlash → Character Combo Profile
```

- Drag your ComboData assets into `availableCombos`

### 3. Wire your existing model (no custom class needed)

If you already have a character model with a rigged Animator, you do **not**
need to write a custom character script. Just attach the combat components
directly to your model's GameObject:

```
YourModel (existing GameObject — mesh + Animator already set up)
├── Animator                  ← already exists on your model
├── ComboController            ← add, assign CharacterComboProfile
├── CombatMediator             ← add, assign an AudioSource
└── PlayerInputHandler         ← add, assign InputActionAsset
```

The only requirement on your Animator Controller: it must have a **Trigger
parameter** for every `animationTrigger` string used in your `AttackStep`
assets (e.g. `"Attack_Light_1"`, `"Attack_Heavy_1"`). `ComboController` calls
`_animator.SetTrigger(step.animationTrigger)` — nothing else about your
Animator setup needs to change.

A custom per-character script is only needed if that character has unique
reactive behavior beyond the shared system — e.g. a fire VFX that only
triggers for one character at 5+ hit combos, or a unique shield on cancel.
In that case, write a small script that subscribes to `ComboController`'s
`IComboEvents` (e.g. `OnHitConfirmed`, `OnComboFinished`) — keep it minimal,
since all core combat logic already lives in `ComboController`.

### 4. Input Action Asset

Create an Action Map named `"Combat"` with:
| Action | Type | Binding |
|---------------|--------|------------------|
| LightAttack | Button | Mouse/Left Button |
| HeavyAttack | Button | Mouse/Right Button |
| SpecialAttack | Button | Keyboard/E |
| Dodge | Button | Keyboard/Space |

### 5. Enemy Setup

Add `DamageReceiver` to any enemy GameObject with a Collider.

---

## Combo Tree Example (Inspector)

```
root
└── step: Light_1  (input: Light)
    └── branches
        ├── [Light] → Light_2
        │   └── branches
        │       ├── [Light]   → Light_3 (finisher)
        │       └── [Heavy]   → LaunchAttack
        └── [Heavy] → Heavy_1
            └── branches
                └── [Special] → GroundSlam  (canCancelIntoSpecial: true)
```

---

## How to Extend

### Add a new character

1. Create a new `CharacterComboProfile` SO
2. Create `ComboData` SOs with their unique branches
3. Attach `ComboController` + `CombatMediator` + `PlayerInputHandler` to the
   character's existing GameObject (see setup step 3 above) — no custom
   class required unless the character needs unique reactive behavior
4. Call `ComboController.SetProfile(newProfile)` to hot-swap at runtime

### Add a new cancel type (e.g. "Parry")

```csharp
_cancelSystem.RegisterRule(new CancelSystem.CancelRule
{
    stepNamePattern  = "*",
    allowedPhase     = ComboPhase.Startup,   // parry window
    cancelTarget     = "Parry"
});
```

### Add a skill tree

- Add `bool isLocked = true` to `ComboData`
- Unlock with `comboData.isLocked = false` from your SkillTree system
- `ComboController.TryStartCombo` already skips locked combos if you
  add the check: `if (combo.isLocked) continue;`

### AI

- `ComboController` has no dependency on `PlayerInputHandler`
- AI can call `comboController.ReceiveInput(AttackInput.Heavy)` directly

---

## Design Principles

| Principle             | How it's applied                                                          |
| --------------------- | ------------------------------------------------------------------------- |
| Data-driven           | Combos live in ScriptableObjects, zero code changes to add new combos     |
| Event-driven          | `IComboEvents` decouples combat from UI/VFX/SFX                           |
| Single Responsibility | Each class does one job                                                   |
| Open/Closed           | Add new cancel rules, characters, combos without modifying core           |
| No hard references    | `CombatMediator` and `ComboUIDisplay` depend on interfaces, not concretes |
