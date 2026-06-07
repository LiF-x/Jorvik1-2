# JorvikMod 1-2

JorvikMod 1 & 2.1 mod pack, made compatible with the **LiFx framework** and **Yo Launcher**.

- Original creator: **Odin one Eye**
- LiFx / Yo Launcher conversion: **Ibun & Mjoed**

This pack adds the Jorvik building/object sets (sheds, bridges, piers, crosses, longhouses,
fortifications, decorator kits, metalworking items, animal statues, heraldry, …) to a
Life is Feudal: Your Own server running the LiFx ServerAutoloader framework, and produces a
matching Yo Launcher client modpack.

---

## Requirements

| Requirement | Where to get it |
|-------------|-----------------|
| A working LiF:YO dedicated server with a MariaDB/MySQL DB (the DB user must be able to `CREATE DATABASE`) | your server host |
| **LiFx framework** — `art.zip` | the `art.zip` asset of the [ServerAutoloader **v4.3.0** release](https://github.com/LiF-x/ServerAutoloader/releases/tag/v4.3.0) |
| A zip tool to build the client pack | [7-Zip](https://7zip.dev/en/download/), or `zip`, or Python |

> **Download the [latest release](https://github.com/LiF-x/Jorvik1-2/releases/latest).** It
> contains the mod source (server side) and a **pre-built `modpack.zip`** ready to upload
> straight to Yo Launcher — no build step needed.
> (Earlier instructions linked `LiF-x/JorvikMod/releases` — that is the old, incompatible
> repo; do not use it.)

---

## Installation

> Back up your server's `data/*.xml` first — this mod overwrites them.

### 1. Install the framework
Put `art.zip` (from ServerAutoloader v4.3.0) in the **server root** (the folder containing
`ddctd_cm_yo_server.exe`). **Do not extract it** — the engine mounts the zip and loads the
framework directly from it.

### 2. Deploy the mod
This repo's layout mirrors the server root. Copy these into the server root:

```
mods/         ->  server mods/         (JorvikMod, JorvikModv2)
data/         ->  server data/         (overwrites vanilla xmls — back up first)
art/          ->  server art/          (heraldry symbols)
yolauncher/   ->  server yolauncher/   (REQUIRED: holds the 3D shapes the mod's datablocks load)
```

`yolauncher/` **must** be present on the server, not just the client — the mod's object
datablocks load their `.dts` shapes from `yolauncher/modpack/mods/Jorvik*/art/...`, and
`JorvikMod2::loadDatablocks` execs `yolauncher/modpack/mods/Jorvik2/art/datablocks/Transport.cs`.

### 3. Enable the data export
Start the server once so the framework creates `mods/AutoloadConfig.cs`, then **stop it** and set:

```cs
// mods/AutoloadConfig.cs
$LiFx::createDataXMLS = true;
```

The **released** `art.zip` ships this as `false`, so it must be turned on by hand.

### 4. Export the data
Start the server. With `createDataXMLS` on, it registers the mod and writes the data XMLs to
`LiFx/dbexport/data/`, then stop it. You will have:

```
LiFx/dbexport/data/recipe.xml
LiFx/dbexport/data/recipe_requirement.xml
LiFx/dbexport/data/objects_types.xml
```

### 5. Distribute the exported data
Copy those three files into **both**:

```
LiFx/dbexport/data/*.xml  ->  data/
LiFx/dbexport/data/*.xml  ->  yolauncher/modpack/data/
```

### 6. Reload the server
Start the server again — **after** copying in step 5, not before — so it imports the new
`data/` and serves the mod data to clients.

> So the order is: **export → copy → reload**. (On a brand-new world a mod object only reaches
> the DB after its `sql/dump.sql` INSERT is applied on the *following* start, so you may need
> one extra start before every object appears in the export.)

### 7. Build the Yo Launcher modpack
Run `createModpack.bat` (needs 7-Zip), which zips everything under `yolauncher\modpack\`
except `*.dso` into `modpack.zip`. Then upload `modpack.zip` to
[Yo Launcher](https://www.yolauncher.app/).

On Linux without 7-Zip, the equivalent is a recursive zip of the **contents** of
`yolauncher/modpack/` excluding `*.dso`.

> **Shortcut:** the `modpack.zip` attached to the [latest release](https://github.com/LiF-x/Jorvik1-2/releases/latest)
> is already built — you can upload it to Yo Launcher directly and skip steps 5–6.

---

## Notes for case-sensitive (Linux) servers/clients

Asset paths are case-sensitive on Linux. The shipped art folders are lowercase
(`art/2D/recipes/`, `art/2D/items/`) — make sure any `ImagePath` you add matches the real
folder case, or icons will silently fail to load on Linux while still working on Windows.

## License

GNU General Public License v3.0 — see [LICENSE](LICENSE).
