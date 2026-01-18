<div align="center">

# Sorting Layer Setter

**Unity package for managing Sorting Layers with sub-layer support**

</div>

---

## Project Info

| Property | Value |
|----------|-------|
| **Developed With** | Unity 6000.2.14f1 |
| **Language** | C# |
| **Platforms** | All platforms supported |

---

## Overview

**Sorting Layer Setter** provides a structured way to manage Unity's sorting layers with an additional **sub-layer** system. This is especially useful for 2D games and UI-heavy projects where you need fine-grained control over render order.

---

## Features

| Feature | Description |
|---------|-------------|
| Custom Attributes | Dropdown selector in Inspector for sorting layers |
| Sub-Layer System | Define sub-layers within each sorting layer |
| Multi-Component | Canvas, SpriteRenderer, ParticleSystem support |
| Validation Tools | Find missing or misconfigured sorting layers |
| Auto-Validation | Validates sorting layers on scene save |

---

## Installation

In the Unity Editor:

1. Open **Window > Package Manager**
2. Click the **+** button
3. Select **Add package from git URL...**
4. Paste this URL:

```text
https://github.com/Otd2/com.otd2.sortinglayersetter.git
```

If you want to install a specific version, append a tag to the URL:

```text
https://github.com/Otd2/com.otd2.sortinglayersetter.git#v1.0.1
```

---

## Usage

### Step 1: Create Config

In the Project window, right-click:

- **Create > OTD2 > Sorting Layer > Config**

> **Important:** Move the config asset under a `Resources` folder:
```text
Assets/Resources/SortingLayerConfig.asset
```

---

### Step 2: Configure Sorting Layers

1.	Select the **SortingLayerConfig** asset
2.	Click **Fetch All Sorting Layers** to copy your project’s current sorting layer list into the config file
3.	Add sub-layers with min/max order ranges
4.	Click **Validate Sorting Orders** to make sure there are no overlapping order ranges

<img width="771" height="1284" alt="Screenshot 2026-01-18 at 22 26 29" src="https://github.com/user-attachments/assets/c8347271-0b1a-4e27-bf17-eee76521bb1c" />

<details>
<summary><b>Example Config Structure</b></summary>

```text
SortingLayerConfig
├── Background
│   ├── Sky (0-99)
│   └── Mountains (100-199)
├── Game
│   ├── Floor (0-49)
│   ├── Characters (50-99)
│   └── Effects (100-149)
└── UI
    ├── Panels (0-99)
    ├── Buttons (100-199)
    └── Tooltips (200-299)
```

</details>

---

### Step 3: Add Components

| Component | Use Case |
|----------|----------|
| `CanvasSortingLayerSetter` | UI Canvas with manual render mode setup |
| `CanvasCameraSortingLayerSetter` | UI Canvas (auto-sets Screen Space - Camera) |
| `SpriteRendererSortingLayerSetter` | 2D Sprites |
| `ParticleSortingLayerSetter` | Particle Systems |

---

### Step 4: Configure in Inspector

#### Valid Configuration
<img width="777" height="186" alt="Screenshot 2026-01-18 at 22 38 25" src="https://github.com/user-attachments/assets/9044de1a-a850-44c8-a49a-5b7a3f7c80dd" />

#### Invalid Configuration
<img width="777" height="210" alt="Screenshot 2026-01-18 at 22 39 01" src="https://github.com/user-attachments/assets/5c3a1b8d-e174-4803-b9e2-8f534d41d53d" />

If there is an error in your scene, Unity will show an error popup when you try to save the scene:

<img width="269" height="290" alt="Screenshot 2026-01-18 at 22 41 20" src="https://github.com/user-attachments/assets/fb22c1ab-7ff2-442b-99ab-010b8f696253" />

---


## Editor Tools

Accessible from the menu:

- **Tools > OTD2 > Sorting Layer**

| Tool | Description |
|------|-------------|
| Find Missing Components | Finds canvases without `SortingLayerSetter` |
| Check All Setters | Validates all setters in prefabs |
| Fix Unknown Layers | Auto-fixes invalid layer references |

---

## Limitations

| # | Limitation | Details |
|---|-----------|---------|
| 1 | Canvas Render Mode | Sorting layers only work with **Screen Space - Camera** and **World Space** render modes. **Screen Space - Overlay** ignores sorting layer settings completely (Unity limitation). Use `CanvasCameraSortingLayerSetter` to auto-set the correct render mode. |
| 2 | Resources Folder | `SortingLayerConfig` asset must be placed in a `Resources` folder for runtime access. |
| 3 | Single Config | Only one `SortingLayerConfig` instance is supported per project. |
| 4 | No Runtime Layers | Sorting layers cannot be created at runtime. This package only manages assignment of existing layers defined in Tag Manager. |

---

<div align="center">

Made by **OTD2**

</div>
