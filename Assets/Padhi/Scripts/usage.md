# 🎧 Sound Manager – Usage Instructions (Unity)

This project uses a centralized Sound Manager.  
**Do NOT** add AudioSources to gameplay objects.  
**Do NOT** call `AudioSource.Play()` anywhere.

**All audio must go through AudioManager.**

---

## 1. Core Rule (Read This First)

- Game scripts **request** sounds.
- The **AudioManager plays** them.

> ⚠️ If you bypass this, you are creating bugs we won't have time to debug.

---

## 2. One-Time Scene Setup (Already Done – Don't Duplicate)

The scene contains:

- **One AudioManager GameObject**
- **Two AudioSources:**
  - `MusicSource` → looping background music
  - `SFXSource` → one-shot sound effects

AudioManager persists across scenes (`DontDestroyOnLoad`).

> 👉 **Do not create another AudioManager. Ever.**

---

## 3. Creating a New Sound

All sounds are **ScriptableObjects**.

### Steps:

1. Right-click in Project window
2. Navigate to `Create → Audio → Sound`
3. Name it clearly:
   - `SFX_PlayerJump`
   - `SFX_ButtonClick`
   - `Music_Level1`

### Configure:

- **Clip** → assign audio clip
- **Volume** → default 0.8–1.0
- **Pitch** → leave at 1 unless intentional
- **Loop**
  - ✅ **ON** → music
  - ❌ **OFF** → sound effects

> 💡 Do not tweak volume in code.

---

## 4. Playing Sound Effects (Most Common)

### Example:

```csharp
public SoundData jumpSound;

void Jump()
{
    AudioManager.Instance.PlaySFX(jumpSound);
}
```

### Rules:

Use `PlaySFX()` for:

- Player actions
- UI clicks
- Interactions
- Feedback sounds

> ❌ Do **NOT** store AudioSources in your script

---

## 5. Playing Music

Music is **exclusive** (one track at a time).

### Example:

```csharp
public SoundData levelMusic;

void Start()
{
    AudioManager.Instance.PlayMusic(levelMusic);
}
```

### Rules:

Only call this when:

- Scene starts
- Game state changes

> ❌ Do **NOT** spam `PlayMusic()` every frame

---

## 6. Volume Control (Global)

Master volume is controlled globally.

```csharp
AudioManager.Instance.SetMasterVolume(value);
```

- `value` range: **0 → 1**
- Use for settings sliders only

> ❌ Do **NOT** change volume per AudioSource

---

## 7. What You Must NOT Do (Zero Exceptions)

- ❌ Add AudioSource to player/enemy/UI objects
- ❌ Call `GetComponent<AudioSource>()`
- ❌ Play sounds inside `Update()`
- ❌ Duplicate AudioManager
- ❌ Hardcode AudioClips in scripts

> Any of the above = **broken audio behavior.**

---

## 8. Naming Conventions (Follow Strictly)

| Type          | Prefix   | Example           |
|---------------|----------|-------------------|
| Sound Effect  | `SFX_`   | `SFX_PlayerDeath` |
| Music         | `Music_` | `Music_MainTheme` |

> If you don't name things clearly, debugging becomes guesswork.

---

## 9. Common Mistakes (And Why They're Bad)

| Problem                       | Cause                                              |
|-------------------------------|---------------------------------------------------|
| Sound plays multiple times    | You're calling Play in `Update()` or `OnTriggerStay` |
| Music restarts randomly       | You're calling `PlayMusic()` more than once       |
| Sound too loud/quiet          | Fix the SoundData, not code                       |

---

## 10. If You Need a New Sound Behavior

**Do NOT hack it locally.**

Ask before adding:

- Positional sound
- Looping SFX
- Fade in/out
- Pausing music

> Random extensions = inconsistent system = chaos.

---

## 11. Philosophy (Why This Exists)

This system exists to:

- Prevent duplicated audio logic
- Keep audio bugs near zero
- Let us focus on gameplay, not plumbing

> If you think "this is slower than just adding an AudioSource"  
> you're thinking locally, not as a team.

---

## ⚠️ Final Warning

If you bypass the Sound Manager:

- **You own the bug**
- **You own the fix**
- **You waste jam time**

**Use the system. Ship the game.**

You own the fix

You waste jam time

Use the system. Ship the game.