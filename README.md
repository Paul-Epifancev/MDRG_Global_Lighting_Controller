# 💡 MDRG Global Lighting Controller

> **Real-time lighting control with persistent settings across all scenes**
> *Управление освещением в реальном времени с сохранением настроек во всех сценах*

**Author / Автор:** Paul.Epifancev (`#warlockforbugs`)  
**Discord:** `anon.jun`  
**Version / Версия:** 1.2.0 (Stable Release / Стабильный релиз)  
**Compatibility / Совместимость:** PC (MelonLoader required / требуется MelonLoader)  
**Hotkeys / Горячие клавиши:** `↑` (UpArrow) to brighten / `↓` (DownArrow) to dim

---

## 📖 About / О проекте

[EN]
**MDRG Global Lighting Controller** is an open-source MelonLoader mod that gives players precise, real-time control over the game's lighting system. Unlike the built-in `SimplyLightScript` (which is tied to the in-game time-of-day timer), this mod allows manual adjustment of light intensity from 0% to 100% in 10% increments, with settings that persist across game restarts.

The controller works across **all scenes**, including those without native light nodes (Shower Scene, Cuddle Scene, Missionary Scene, Shop Scene, Clothier Scene), by dynamically injecting light nodes at runtime via IL2CPP reflection.

[RU]
**MDRG Global Lighting Controller** — это open-source мод на базе MelonLoader, который даёт игрокам точный контроль над системой освещения игры в реальном времени. В отличие от встроенного `SimplyLightScript` (который привязан к внутриигровому таймеру времени суток), этот мод позволяет вручную регулировать интенсивность света от 0% до 100% с шагом 10%, с сохранением настроек между перезапусками игры.

Контроллер работает **во всех сценах**, включая те, где нет нативных нод освещения (Shower Scene, Cuddle Scene, Missionary Scene, Shop Scene, Clothier Scene), динамически инжектируя ноды в рантайме через IL2CPP-рефлексию.
<p align="center">
  <img src="light up down.png" alt="MDRG Custom Mod Showcase" width="700">
</p>

---

## ✨ Key Features / Ключевые возможности

- 🎯 **Manual Control / Ручное управление:** Press `↑` to brighten (+10%) or `↓` to dim (-10%) / Нажмите `↑` для увеличения (+10%) или `↓` для уменьшения (-10%)
- 💾 **Persistent Settings / Сохранение настроек:** Light level is saved to `light_level.txt` and restored on game restart / Уровень света сохраняется в `light_level.txt` и восстанавливается при перезапуске
- 🔄 **Scene-Aware Injection / Инжект по сценам:** Works in scenes without native light nodes (Shower, Cuddle, Missionary, Shop, Clothier) / Работает в сценах без нативных нод (Shower, Cuddle, Missionary, Shop, Clothier)
- 🧠 **Live2D Support / Поддержка Live2D:** Automatically adjusts Live2D character lighting via safe reflection / Автоматически регулирует освещение Live2D-персонажей через безопасную рефлексию
- ⚡ **Optimized Caching / Оптимизированное кэширование:** Caches all `Light`, `SpriteRenderer`, and `CanvasGroup` components for minimal CPU overhead / Кэширует все `Light`, `SpriteRenderer` и `CanvasGroup` для минимальной нагрузки на ЦП
- 🛡️ **Safe Clamp / Безопасный диапазон:** Strict 0%–100% range with 10% steps / Строгий диапазон 0%–100% с шагом 10%

---

## 🚀 Quick Start / Быстрый старт

[EN]
1. Make sure you have [MelonLoader](https://github.com/LavaGang/MelonLoader) installed for MDRG.
2. Download `GlobalLightingController.dll` from the [Releases](../../releases) section.
3. Place it in your game's `Mods/` folder.
4. Launch the game.
5. Press `↑` to brighten or `↓` to dim. Settings are saved automatically.

[RU]
1. Убедитесь, что у вас установлен [MelonLoader](https://github.com/LavaGang/MelonLoader) для MDRG.
2. Скачайте `GlobalLightingController.dll` из раздела [Releases](../../releases).
3. Поместите его в папку `Mods/` в директории вашей игры.
4. Запустите игру.
5. Нажмите `↑` для увеличения яркости или `↓` для уменьшения. Настройки сохраняются автоматически.

---

## ⚙️ How It Works / Как это работает

[EN]
The mod uses a multi-layered approach to control lighting:

1. **Room Node Cache:** On scene load, it scans all child objects under `/Room` for `Light`, `SpriteRenderer`, and `CanvasGroup` components matching specific paths (e.g., `Light Multiply`, `Morning Light`, `TV/TV light`).
2. **Deep Scan:** It also scans all `SpriteRenderer` objects in the scene by name keywords (`light`, `multiply`, `window`, `rgb`, `screen`, etc.) to catch dynamic objects.
3. **Live2D Integration:** Uses IL2CPP reflection to access `Live2DControllerSingleton.Instance.AllActiveControllers` and adjusts all child renderers of active Live2D models.
4. **Scene Injection:** For scenes without native light nodes (Cuddle, Shower, Missionary, Shop, Clothier), it dynamically finds target GameObjects by full transform path and applies color modulation.
5. **Persistence:** Light level is saved to `light_level.txt` in the game's root directory and restored on startup.

[RU]
Мод использует многоуровневый подход к управлению освещением:

1. **Кэш нод комнаты:** При загрузке сцены сканирует все дочерние объекты под `/Room` на наличие компонентов `Light`, `SpriteRenderer` и `CanvasGroup`, соответствующих определённым путям (например, `Light Multiply`, `Morning Light`, `TV/TV light`).
2. **Глубокое сканирование:** Также сканирует все `SpriteRenderer` в сцене по ключевым словам в именах (`light`, `multiply`, `window`, `rgb`, `screen` и т.д.) для перехвата динамических объектов.
3. **Интеграция Live2D:** Использует IL2CPP-рефлексию для доступа к `Live2DControllerSingleton.Instance.AllActiveControllers` и регулирует все дочерние рендереры активных Live2D-моделей.
4. **Инжект по сценам:** Для сцен без нативных нод (Cuddle, Shower, Missionary, Shop, Clothier) динамически находит целевые GameObject по полному пути трансформации и применяет модуляцию цвета.
5. **Сохранение:** Уровень света сохраняется в `light_level.txt` в корневой директории игры и восстанавливается при запуске.

---

## 🛡️ Safety Guarantees / Гарантии безопасности

✅ **No save file modification** — only adjusts rendering parameters in RAM / Не модифицирует файлы сохранений — регулирует только параметры рендеринга в RAM  
✅ **No original game files modified** — pure MelonLoader injection, 100% reversible / Оригинальные файлы игры не изменяются — чистая инъекция через MelonLoader, 100% обратимо  
✅ **Safe Live2D reflection** — uses try-catch blocks to prevent crashes on missing types / Безопасная Live2D-рефлексия — использует try-catch для предотвращения крашей при отсутствии типов  
✅ **Optimized performance** — caches components to avoid per-frame scanning / Оптимизированная производительность — кэширует компоненты, чтобы избежать сканирования каждый кадр  

---

## ⚠️ Known Limitations / Известные ограничения

- **10% step increments** — fine-grained control (e.g., 1% steps) is not currently supported / Шаг 10% — точный контроль (например, шаг 1%) пока не поддерживается
- **Console feedback** — light level changes are logged to the MelonLoader console / Изменения уровня света логируются в консоль MelonLoader
- **PC only** — MelonLoader-based, not compatible with Android/mobile versions / Только PC — на базе MelonLoader, не совместим с Android/мобильными версиями

---

## 🔧 Tech Stack / Технологии

- **Language / Язык:** C# (.NET 6.0)
- **Framework / Фреймворк:** [MelonLoader](https://github.com/LavaGang/MelonLoader)
- **Target / Цель:** Unity IL2CPP (MDRG)
- **APIs:** IL2CPP Reflection (`Il2CppInterop.Runtime`), `UnityEngine.Light`, `UnityEngine.SpriteRenderer`, `UnityEngine.CanvasGroup`
- **Persistence / Сохранение:** File I/O (`light_level.txt`)

---

## 💬 Community & Support / Сообщество и поддержка

🔗 **Discord Server / Discord-сервер:** [discord.gg/nwcvMUZgM](https://discord.gg/nwcvMUZgM)  
🔗 **Main Toolkit Repository / Репозиторий основного тулкита:** [MDRG-Modding-Toolkit](https://github.com/Paul-Epifancev/MDRG-Modding-Toolkit)  
🔗 **Inventory Cleaner Repository / Репозиторий Inventory Cleaner:** [MDRG-Inventory-Cleaner](https://github.com/Paul-Epifancev/MDRG-Inventory-Cleaner)

---

## 👤 About the Author / Об авторе

[EN]
This project is created by **Paul.Epifancev** (`#warlockforbugs`). My journey in game modification began with 5 years of deep engine analysis in World of Warcraft (2013–2018). Today, I apply the same analytical approach and reverse-engineering skills to Unity/IL2CPP environments.

[RU]
Этот проект создан **Paul.Epifancev** (`#warlockforbugs`). Мой путь в модификации игр начался с пяти лет глубокого анализа движка в World of Warcraft (2013–2018). Сегодня я применяю тот же аналитический подход и навыки реверс-инжиниринга к среде Unity/IL2CPP.

---

## 📄 License / Лицензия

This project is released under the MIT License. See [LICENSE](LICENSE) for details.

---

## ⚠️ Disclaimer / Отказ от ответственности

This mod is provided **as-is** for educational and aesthetic purposes. It does not modify original game files on disk. All lighting changes are applied in RAM and are fully reversible. **Always backup your save files before using any mods.**

---

**Made with ❤️ and 🐑 by Paul.Epifancev (`#warlockforbugs`)**  
*«Не пойман — не багер»
