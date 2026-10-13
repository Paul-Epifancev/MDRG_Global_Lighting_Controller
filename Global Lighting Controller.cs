using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using Il2CppInterop.Runtime;

[assembly: MelonInfo(typeof(GlobalLightingController.MyLightingMod), "Global Lighting Controller", "1.2.0", "Paul.Epifancev #warlockforbugs (Discord: anon.jun)")]
[assembly: MelonGame(null, null)]

namespace GlobalLightingController
{
	public class MyLightingMod : MelonMod
	{
		public static MyLightingMod Instance { get; private set; }

		private static float _currentIntensity = 1f;
		private float _lastIntensity = -1f;
		private bool _cacheInitialized;
		private float _initTimer;
		private const float _initDelay = 0.3f; 
		private float _actualRenderIntensity = 1f; 

		private static string _configPath;

		private readonly List<Light> _cachedLights = new List<Light>();
		private readonly List<SpriteRenderer> _cachedRenderers = new List<SpriteRenderer>();
		private readonly List<CanvasGroup> _cachedCanvasGroups = new List<CanvasGroup>();

		private static System.Type _l2dSingletonType;
		private static System.Reflection.PropertyInfo _l2dInstanceProp;
		private static System.Reflection.PropertyInfo _l2dActiveControllersProp;
		private static System.Reflection.PropertyInfo _l2dGameObjectProp;
		private static bool _l2dReflectionSuccess;

		private readonly string[] _targetRoomChildPaths = new string[]
		{
			"back", "Door", "mat", "GhostBackground", "couch", "SpiderPlush", "Cupboard", "Bed", 
			"BatBanner", "Window", "TV/TV", "TV/TV light", "Bathroom Door/Door", "Bathroom Door/Bathroom Door Light",
			"PC/PC desk", "PC/PC", "PC/Screen light", "PC/PC rgb", "Light Multiply", "Light", "Morning Light"
		};

		public override void OnInitializeMelon()
		{
			Instance = this;
			
			MelonLogger.Msg("==================================================");
			MelonLogger.Msg("===   GLOBAL LIGHTING SYNC ACTIVE (v1.2.0)     ===");
			MelonLogger.Msg(" Strict Clamp (0% - 100%) with 10% steps active! ");
			MelonLogger.Msg(" Press [ UpArrow ] to Brighten | [ DownArrow ] to Dim. ");
			MelonLogger.Msg(" Safe Dynamic Live2D Reflector Active!            ");
			MelonLogger.Msg("==================================================");

			_configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "light_level.txt");
			LoadLightLevelFromDisk();

			_actualRenderIntensity = _currentIntensity;

			try
			{
				_l2dSingletonType = System.Type.GetType("Live2DControllerSingleton, Assembly-CSharp")
								  ?? System.Type.GetType("Il2Cpp.Live2DControllerSingleton, Assembly-CSharp")
								  ?? System.Type.GetType("Live2DControllerSingleton, Il2Cpp__Generated");

				if (_l2dSingletonType != null)
				{
					_l2dInstanceProp = _l2dSingletonType.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
					if (_l2dInstanceProp != null)
					{
						object instance = _l2dInstanceProp.GetValue(null);
						if (instance != null)
						{
							_l2dActiveControllersProp = instance.GetType().GetProperty("AllActiveControllers", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
							if (_l2dActiveControllersProp == null) _l2dActiveControllersProp = instance.GetType().GetProperty("AllActiveControllers", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
							
							var controllers = _l2dActiveControllersProp?.GetValue(instance) as System.Collections.IEnumerable;
							if (controllers != null)
							{
								foreach (var controller in controllers)
								{
									if (controller != null)
									{
										_l2dGameObjectProp = controller.GetType().GetProperty("gameObject", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
										_l2dReflectionSuccess = (_l2dGameObjectProp != null);
										break;
									}
								}
							}
						}
					}
				}
			}
			catch { _l2dReflectionSuccess = false; }
		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			this._lastIntensity = -1f;
			this._cacheInitialized = false;
			this._initTimer = 0f;
			
			this._actualRenderIntensity = _currentIntensity; 

			this._cachedLights.Clear();
			this._cachedRenderers.Clear();
			this._cachedCanvasGroups.Clear();
			
			try { this.ForceDeepCacheScan(); } catch { }
		}
		public override void OnUpdate()
		{
			if (!this._cacheInitialized)
			{
				this._initTimer += Time.deltaTime;
				_actualRenderIntensity = _currentIntensity;
				this.ApplyDynamicLightingFast(false);

				if (this._initTimer >= _initDelay)
				{
					this.ForceDeepCacheScan();
					this._cacheInitialized = true;
				}
				return;
			}

			_actualRenderIntensity = _currentIntensity;

			try
			{
				ForceInjectNodeContinuous("/Views/Cuddle Scene/Cuddle Background", "Cuddle Background");
				ForceInjectNodeContinuous("/Views/Cuddle Scene/Cuddle Bed", "Cuddle Bed");
				ForceInjectNodeContinuous("/Views/Cuddle Scene/Light Multiply", "Light Multiply");
				ForceInjectNodeContinuous("/Views/Cuddle Scene/LightSonic", "LightSonic");

				ForceInjectNodeContinuous("/Views/Shower Scene/Foreground/Shower Foreground", "Shower Foreground");
				ForceInjectNodeContinuous("/Views/Shower Scene/Center/Shower Wall", "Shower Wall");
				ForceInjectNodeContinuous("/Views/Shower Scene/Back/Shower Back", "Shower Back");
				ForceInjectNodeContinuous("/Views/Shower Scene/Back/Shower Back Props", "Shower Back Props");

				ForceInjectNodeContinuous("/Views/Missionary Scene/Missionary Image", "Missionary Image");
				ForceInjectNodeContinuous("/Views/Missionary Scene/Light Multiply", "Light Multiply");

				ForceInjectNodeContinuous("/Views/Shop Scene", "Shop Scene");
				ForceInjectNodeContinuous("/Views/Clothier Scene", "Clothier Scene");
			}
			catch { }

			bool forceLog = false;

			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				if (_currentIntensity >= 1f)
				{
					MelonLogger.Msg("[Lighting] Light is already at maximum (100%).");
				}
				else
				{
					_currentIntensity = Mathf.Min(1f, Mathf.Round((_currentIntensity + 0.1f) * 10f) / 10f);
					forceLog = true;
					SaveLightLevelToDisk();
				}
			}

			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				if (_currentIntensity <= 0f)
				{
					MelonLogger.Msg("[Lighting] Light is already at minimum (0%).");
				}
				else
				{
					_currentIntensity = Mathf.Max(0f, Mathf.Round((_currentIntensity - 0.1f) * 10f) / 10f);
					forceLog = true;
					SaveLightLevelToDisk();
				}
			}

			this.ApplyDynamicLightingFast(forceLog);
		}
		private void ForceDeepCacheScan()
		{
			try
			{
				this._cachedLights.Clear();
				this._cachedRenderers.Clear();
				this._cachedCanvasGroups.Clear();

				GameObject roomGo = GameObject.Find("/Room") ?? GameObject.Find("Room");
				if (roomGo != null && roomGo.transform != null)
				{
					Transform roomTransform = roomGo.transform;
					for (int i = 0; i < this._targetRoomChildPaths.Length; i++)
					{
						Transform childTransform = roomTransform.Find(this._targetRoomChildPaths[i]);
						if (childTransform != null && childTransform.gameObject != null)
						{
							Light component = childTransform.gameObject.GetComponent<Light>();
							if (component != null) this._cachedLights.Add(component);
							
							SpriteRenderer component2 = childTransform.gameObject.GetComponent<SpriteRenderer>();
							if (component2 != null) this._cachedRenderers.Add(component2);
							
							CanvasGroup component3 = childTransform.gameObject.GetComponent<CanvasGroup>();
							if (component3 != null) this._cachedCanvasGroups.Add(component3);
						}
					}
				}

				var allSprites = Resources.FindObjectsOfTypeAll(Il2CppType.Of<SpriteRenderer>());
				for (int i = 0; i < allSprites.Length; i++)
				{
					SpriteRenderer spriteRenderer = allSprites[i].TryCast<SpriteRenderer>();
					if (spriteRenderer == null || spriteRenderer.gameObject == null) continue;

					string text = spriteRenderer.gameObject.name.ToLower();
					if (text.Contains("light") || text.Contains("multiply") || text.Contains("window") || 
						text.Contains("rgb") || text.Contains("screen") || text.Contains("image") || 
						text.Contains("layer") || text.Contains("texture") || text.Contains("missionary") || 
						text.Contains("shower") || text.Contains("blowjob") || text.Contains("cowgirl") || 
						text.Contains("doggy") || text.Contains("interaction") || text.Contains("kitchen") ||
						text.Contains("background") || text.Contains("door") || text.Contains("mat") || text.Contains("couch"))
					{
						if (!this._cachedRenderers.Contains(spriteRenderer)) this._cachedRenderers.Add(spriteRenderer);
					}
				}

				if (_l2dReflectionSuccess && _l2dInstanceProp != null)
				{
					try
					{
						object singletonInstance = _l2dInstanceProp.GetValue(null);
						if (singletonInstance != null && _l2dActiveControllersProp != null)
						{
							var controllers = _l2dActiveControllersProp.GetValue(singletonInstance) as System.Collections.IEnumerable;
							if (controllers != null)
							{
								foreach (var controller in controllers)
								{
									if (controller == null) continue;
									GameObject controllerGo = _l2dGameObjectProp.GetValue(controller) as GameObject;
									if (controllerGo != null)
									{
										var childRenders = controllerGo.GetComponentsInChildren<SpriteRenderer>(true);
										for (int j = 0; j < childRenders.Length; j++)
										{
											SpriteRenderer render = childRenders[j];
											if (render != null && !this._cachedRenderers.Contains(render)) this._cachedRenderers.Add(render);
										}
									}
								}
							}
						}
					}
					catch { }
				}

				var allLights = Resources.FindObjectsOfTypeAll(Il2CppType.Of<Light>());
				for (int i = 0; i < allLights.Length; i++)
				{
					Light light = allLights[i].TryCast<Light>();
					if (light != null && !this._cachedLights.Contains(light)) this._cachedLights.Add(light);
				}
			}
			catch (Exception ex) { MelonLogger.Error("[-] Cache Builder Failure: " + ex.Message); }
		}

		private void ForceInjectNodeContinuous(string fullTransformPath, string localName)
		{
			try
			{
				GameObject targetGo = GameObject.Find(fullTransformPath) ?? GameObject.Find(localName);
				if (targetGo != null)
				{
					SpriteRenderer renderer = targetGo.GetComponent<SpriteRenderer>();
					if (renderer == null)
					{
						var rawComponent = targetGo.GetComponentInChildren(Il2CppType.Of<SpriteRenderer>());
						if (rawComponent != null) renderer = rawComponent.TryCast<SpriteRenderer>();
					}

					if (renderer != null)
					{
						renderer.color = new Color(_actualRenderIntensity, _actualRenderIntensity, _actualRenderIntensity, 1f);
						if (!this._cachedRenderers.Contains(renderer)) this._cachedRenderers.Add(renderer);
					}
				}
			}
			catch { }
		}

		private void ApplyDynamicLightingFast(bool forceLog)
		{
			try
			{
				for (int i = 0; i < this._cachedLights.Count; i++)
				{
					Light light = this._cachedLights[i];
					if (light != null) light.intensity = _actualRenderIntensity;
				}

				Color targetColor = new Color(_actualRenderIntensity, _actualRenderIntensity, _actualRenderIntensity, 1f);

				for (int j = 0; j < this._cachedRenderers.Count; j++)
				{
					SpriteRenderer renderer = this._cachedRenderers[j];
					if (renderer != null) renderer.color = targetColor;
				}

				for (int k = 0; k < this._cachedCanvasGroups.Count; k++)
				{
					CanvasGroup group = this._cachedCanvasGroups[k];
					if (group != null) group.alpha = _actualRenderIntensity;
				}

				if (forceLog)
				{
					MelonLogger.Msg($"[Lighting Sync] Intensity set to: {Mathf.RoundToInt(_currentIntensity * 100f)}%");
				}

				this._lastIntensity = _currentIntensity;
			}
			catch { }
		}

		private static void SaveLightLevelToDisk()
		{
			try { File.WriteAllText(_configPath, _currentIntensity.ToString("F1")); } catch { }
		}

		private static void LoadLightLevelFromDisk()
		{
			try
			{
				if (File.Exists(_configPath))
				{
					string content = File.ReadAllText(_configPath).Trim();
					if (float.TryParse(content, out float parsedVal))
					{
						_currentIntensity = Mathf.Clamp(parsedVal, 0f, 1f);
					}
				}
			}
			catch { }
		}
	}
}
