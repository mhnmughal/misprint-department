# AUDIO_PROVENANCE — Misprint Department

Every audio asset is **project-generated original**, synthesised in the
editor by `Assets/Scripts/Editor/Build/AudioBuilder.cs`. Method: struck-partial
voices (summed decaying sines), filtered pseudo-noise (xorshift RNG,
deterministic seeds), one-pole low-pass, smooth attack + exponential-release
envelopes, and per-clip peak normalisation with DC removal. 16-bit PCM WAV
@ 44.1 kHz mono. Regenerate with menu `Misprint/Build/2 · Audio`.

External dependency: **None.** No file was downloaded, sampled, or derived
from any commercial or third-party recording.

Files live in `Assets/Audio/SFX/`. Mixed through `Assets/Audio/MisprintMixer.mixer`
(groups Master → Music / SFX / UI; exposed params MasterVolume, MusicVolume,
SFXVolume, UIVolume). Mapped to gameplay events by `Assets/Audio/SoundBank.asset`.

| Filename | Purpose | Creation method | Source | License |
|----------|---------|-----------------|--------|---------|
| ui_select.wav | UI selection blip | synth: 660 Hz tone + fast envelope | Project-generated original | Original — project owned |
| mech_button.wav | Mechanical button press | synth: 180 Hz tone + noise transient | Project-generated original | Original — project owned |
| correction_rotate.wav | Rotate correction (ratchet) | synth: 3 rising filtered-noise ticks | Project-generated original | Original — project owned |
| correction_replace.wav | Replace correction (stamp) | synth: 140 Hz thunk + paper noise | Project-generated original | Original — project owned |
| printer_start.wav | Printer motor spin-up | synth: rising sawtooth+harmonic + noise bed | Project-generated original | Original — project owned |
| roller_feed.wav | Roller / paper feed | synth: low-passed noise + periodic clicks | Project-generated original | Original — project owned |
| proof_lever.wav | Run Proof lever throw | synth: pull noise + 95 Hz clunk + latch tick | Project-generated original | Original — project owned |
| route_step.wav | Carriage advances one cell | synth: 240 Hz short tone | Project-generated original | Original — project owned |
| route_turn.wav | Carriage turns | synth: 300→520 Hz pitch sweep | Project-generated original | Original — project owned |
| proof_approved.wav | Job approved | synth: two struck partials (523/784 Hz) + clack | Project-generated original | Original — project owned |
| proof_rejected.wav | Reprint required | synth: 92 Hz square buzzer + 70 Hz thud | Project-generated original | Original — project owned |
| ticket_move.wav | Job ticket handled | synth: shaped noise sweep | Project-generated original | Original — project owned |
| stamp_mark.wav | QC stamp impact | synth: 110 Hz hit + noise snap | Project-generated original | Original — project owned |
| ambience_bench.wav | Workshop ambience loop (4 s) | synth: 50/100 Hz hum + slow-drift noise, edge-faded | Project-generated original | Original — project owned |

## Typography
`LiberationSans SDF` (TextMeshPro essential resources). Liberation Sans is
licensed under the **SIL Open Font License 1.1** — free for commercial use
and redistribution. A bespoke industrial display face may be added later; it
must carry documented commercial-use licensing before shipping.
