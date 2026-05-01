# Fear Transfer Library
# Format:
# event = bus;profile;material;fear;gainDb;notes
#
# The engine-level AudioParams fields mirror these presets. This file is the authored transfer table used by
# content reviewers to keep fear-inducing audio choices consistent across emotes, weapons, ambience and UI.

scream = Voice;NonlinearVocalization;Flesh;0.85;0;chaotic pitch micro-modulation, sub-harmonic roughness band
howl = Voice;NonlinearVocalization;Flesh;0.80;-1;non-linear vocal fold saturation profile
cough = Voice;BreathCough;Flesh;0.55;-2;near-field glottal intimacy and chest-cavity coloration
gasp = Voice;BreathCough;Flesh;0.50;-1;short inspiratory transient with proximity boost
whisper = Voice;WhisperOccluded;Flesh;0.45;-4;lip-radiation attenuation, nasal notch, turbulent high band
gunshot = Sfx;BallisticCrack;Metal;0.45;0;muzzle blast plus projectile crack priority routing
suppressed-gunshot = Sfx;SuppressedWeapon;Metal;0.35;-5;low-pass cascade, gas pulse reduction, mechanical tail
looming-step = Sfx;LoomingThreat;Concrete;0.70;-2;ascending gain and pitch bias for unseen approach
startle-burst = Sfx;StartleBurst;Metal;1.00;3;short broadband transient with compressor bypass
vent-voice = Voice;VentResonance;Metal;0.55;-6;comb delay derived from pipe length and air column resonance
wall-transmission = Sfx;StructuralTransmission;Concrete;0.35;-8;mass-law low-pass for shared-wall propagation
fear-drone = FearLayer;InfrasoundCarrier;OpenField;0.65;-40;coherent low-frequency carrier under ambience
shepard-fall = FearLayer;ProceduralMusic;OpenField;0.75;-36;descending Shepard-Risset tension layer
