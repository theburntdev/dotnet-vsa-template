# dotnet new Template Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Register this repo as a `dotnet new` template so users can scaffold a new VSA project via `dotnet new dotnet-vsa -n MyProject`.

**Architecture:** A `.template.config/template.json` at the repo root configures the .NET template engine. The `sourceName` field (`BackendTemplate`) is the only rename token — the engine auto-replaces it (and all casing variants) with the user-supplied `-n` value everywhere in file names and file content. No extra parameters or scripting needed.

**Tech Stack:** .NET Template Engine (`dotnet new`), JSON

---

## File Map

| Action | Path | Purpose |
|--------|------|---------|
| Create | `.template.config/template.json` | Template engine config: identity, short name, sourceName, exclusions, post-action |
| Modify | `README.md` | Add "Using this template" section with install + usage commands |

---

### Task 1: Create `.template.config/template.json`

**Files:**
- Create: `.template.config/template.json`

- [ ] **Step 1: Create the config directory and template.json**

From repo root:

```powershell
New-Item -ItemType Directory -Path ".template.config" -Force
```

Then create `.template.config/template.json` with this exact content:

```json
{
  "$schema": "http://json.schemastore.org/template",
  "author": "Brian Jong",
  "classifications": ["Web", "WebAPI", "Vertical Slice"],
  "identity": "DotnetVsa.Template",
  "name": "Dotnet VSA Template",
  "shortName": "dotnet-vsa",
  "sourceName": "BackendTemplate",
  "preferNameDirectory": true,
  "defaultName": "MyApp",
  "tags": {
    "language": "C#",
    "type": "project"
  },
  "sources": [
    {
      "modifiers": [
        {
          "exclude": [
            "docs/**"
          ]
        }
      ]
    }
  ],
  "postActions": [
    {
      "id": "restore",
      "description": "Restore NuGet packages",
      "actionId": "210D431B-A78B-4D2F-B762-4DC3F3AE9B31",
      "continueOnError": true
    }
  ]
}
```

- [ ] **Step 2: Install the template locally**

From repo root:

```powershell
dotnet new install .
```

Expected output (exact short name must appear):

```
The following template packages will be installed:
   <path-to-repo>

Success: <path-to-repo> installed the following templates:
Template Name         Short Name   Language  Tags
--------------------  -----------  --------  ----------------------------
Dotnet VSA Template   dotnet-vsa   C#        Web/WebAPI/Vertical Slice
```

- [ ] **Step 3: Verify template appears in list**

```powershell
dotnet new list dotnet-vsa
```

Expected output:

```
These templates matched your input: 'dotnet-vsa'

Template Name         Short Name   Language  Tags
--------------------  -----------  --------  ----------------------------
Dotnet VSA Template   dotnet-vsa   C#        Web/WebAPI/Vertical Slice
```

- [ ] **Step 4: Generate a test project**

```powershell
dotnet new dotnet-vsa -n Acme.Orders -o C:\Temp\Acme.Orders
```

Expected output:

```
The template "Dotnet VSA Template" was created successfully.

Processing post-creation actions...
Restoring C:\Temp\Acme.Orders\backend\src\Acme.Orders.Api\Acme.Orders.Api.csproj:
  ...
Restore succeeded.
```

- [ ] **Step 5: Verify rename correctness**

Check that `BackendTemplate` does not appear anywhere in the generated output:

```powershell
Select-String -Path "C:\Temp\Acme.Orders\**\*" -Pattern "BackendTemplate" -Recurse
```

Expected: **no matches**

Also verify key renames happened:

```powershell
Test-Path "C:\Temp\Acme.Orders\backend\Acme.Orders.slnx"
Test-Path "C:\Temp\Acme.Orders\backend\src\Acme.Orders.Api\Acme.Orders.Api.csproj"
```

Expected: both return `True`

- [ ] **Step 6: Verify generated project builds**

```powershell
dotnet build "C:\Temp\Acme.Orders\backend"
```

Expected:

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

- [ ] **Step 7: Verify docs/ was excluded**

```powershell
Test-Path "C:\Temp\Acme.Orders\docs"
```

Expected: `False`

- [ ] **Step 8: Clean up test output**

```powershell
Remove-Item -Recurse -Force "C:\Temp\Acme.Orders"
```

- [ ] **Step 9: Commit**

```powershell
git add .template.config/template.json
git commit -m "feat: add dotnet new template config (dotnet-vsa)"
```

---

### Task 2: Update README with template usage instructions

**Files:**
- Modify: `README.md`

- [ ] **Step 1: Add "Using this template" section**

Insert the following block into `README.md` immediately after the opening description line (`Modern Vertical Slice Architecture backend template...`) and before `## Prerequisites`:

```markdown
## Using this template

Install the template from a local clone of this repo:

```powershell
dotnet new install .
```

Scaffold a new project (replace `Acme.Orders` with your project name):

```powershell
dotnet new dotnet-vsa -n Acme.Orders
cd Acme.Orders
```

All occurrences of `BackendTemplate` in file names, namespaces, and content are replaced with your project name automatically. Follow the [Local setup](#local-setup) steps below to get the generated project running.

To uninstall the template:

```powershell
dotnet new uninstall .
```
```

- [ ] **Step 2: Verify README renders correctly**

Read `README.md` and confirm:
- "Using this template" section appears before "Prerequisites"
- All code blocks are properly closed
- Commands reference `dotnet-vsa` short name

- [ ] **Step 3: Commit**

```powershell
git add README.md
git commit -m "docs: add dotnet new template usage instructions to README"
```
