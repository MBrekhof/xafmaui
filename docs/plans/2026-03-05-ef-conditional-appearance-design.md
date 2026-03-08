# EF-Only Conditional Appearance for XAF Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Build an EF Core-only XAF module that lets administrators define conditional appearance rules in the database and apply them at runtime without redeploying.

**Architecture:** Use a two-project module split: a shared runtime/core project for rule translation and controller orchestration, plus an EF project for persistence and XAF module registration. Rules are stored as EF entities, loaded through a provider service, translated into runtime appearance objects, cached per type/context, and invalidated on data changes.

**Tech Stack:** .NET 8+, DevExpress XAF v25.2, EF Core, Blazor/Win XAF UI as applicable, xUnit (or NUnit) for automated tests.

---

## Scope and Assumptions

- This plan targets a new XAF solution, not the current MAUI sample app.
- Objective is an EF-only implementation with strong maintainability, not a byte-for-byte clone.
- Some behavior details are inferred from package metadata/symbol names because the vendor docs page was unavailable at the time of analysis.

## Design Summary

### Recommended approach

1. Build a clean, provider-based architecture (`IAppearanceRuleDataProvider` + `IRuleTranslator`) and keep EF specifics out of runtime rule creation logic.
2. Start with a focused MVP field set and rule semantics.
3. Add UX and parity features incrementally once tests stabilize.

### Alternatives considered

1. Monolithic single project (faster start, harder to test/extend).
2. Split shared+EF projects (recommended: clear boundaries, easier tests, reusable runtime logic).
3. Heavy model-layer customization first (high initial complexity, slower delivery).

## Target Project Structure

```text
src/
  MyCompany.Module.ConditionalAppearance/
    MyCompany.Module.ConditionalAppearance.csproj
    Module/
      ConditionalAppearanceModuleBase.cs
    Controllers/
      ConditionalAppearanceCollectorController.cs
      ConditionalAppearanceCacheController.cs
    Services/
      IAppearanceRuleDataProvider.cs
      AppearanceRuleTranslator.cs
      AppearanceRuleCache.cs
    Contracts/
      IAppearanceRuleData.cs
      IAppearanceRuleProperties.cs
    Runtime/
      RuntimeAppearanceRule.cs
      RuntimeAppearanceRuleFactory.cs
    Utilities/
      AppearanceRuleValidation.cs
      AppearanceTypeResolver.cs

  MyCompany.Module.ConditionalAppearance.EF/
    MyCompany.Module.ConditionalAppearance.EF.csproj
    Module/
      ConditionalAppearanceEFModule.cs
      ConditionalAppearanceEFModuleBuilderExtensions.cs
    BusinessObjects/
      AppearanceRuleData.cs
      AppearanceRuleContext.cs
      AppearanceRuleTargetTypeMap.cs
    Services/
      EFAppearanceRuleDataProvider.cs
    Model/
      ModelNodesGeneratorUpdater.cs
      ConditionalAppearanceModelDefaults.cs

tests/
  MyCompany.Module.ConditionalAppearance.Tests/
    MyCompany.Module.ConditionalAppearance.Tests.csproj
    Unit/
      AppearanceRuleTranslatorTests.cs
      AppearanceRuleValidationTests.cs
      AppearanceRuleCacheTests.cs
  MyCompany.Module.ConditionalAppearance.EF.Tests/
    MyCompany.Module.ConditionalAppearance.EF.Tests.csproj
    Integration/
      EFAppearanceRuleDataProviderTests.cs
      RuleApplicationFlowTests.cs
```

## Data Model (MVP)

Entity: `AppearanceRuleData`

- `Id` (`Guid` or `int`)
- `Name` (`string`, unique per target type optional)
- `IsEnabled` (`bool`)
- `DataTypeName` (`string`) - full type name
- `Context` (`string`) - list/detail/any custom context
- `Criteria` (`string`) - XAF criteria expression
- `TargetItems` (`string`) - semicolon/comma separated property names
- `Priority` (`int`)
- `Method` (`string?`) - optional callback method name
- `Visibility` (`enum?`) - show/hide
- `BackColorValue` (`int?`)
- `FontColorValue` (`int?`)
- `FontStyle` (`enum?`)
- `UpdatedOnUtc` (`DateTime`)

## Runtime Flow

1. XAF view/controller triggers collection of appearance rules.
2. Cache queried by `(ObjectType, Context, ViewKind)`.
3. On cache miss, provider loads enabled rule rows from EF.
4. Translator validates each row and creates runtime appearance entries.
5. Runtime rules are handed to XAF conditional appearance pipeline.
6. Cache invalidates when rules are created/updated/deleted.

## Error Handling and Guardrails

1. Invalid `DataTypeName`: skip rule, log warning with rule id/name.
2. Invalid criteria syntax: skip rule, log parser error.
3. Missing `TargetItems`: allow object-level rule when supported, else log/skip.
4. Unknown context: skip by default unless explicit fallback configured.
5. Cache corruption suspicion: force rebuild and emit diagnostic event.

## Task Plan

### Task 1: Solution and project bootstrap

**Files:**
- Create: `src/MyCompany.Module.ConditionalAppearance/MyCompany.Module.ConditionalAppearance.csproj`
- Create: `src/MyCompany.Module.ConditionalAppearance.EF/MyCompany.Module.ConditionalAppearance.EF.csproj`
- Create: `tests/MyCompany.Module.ConditionalAppearance.Tests/MyCompany.Module.ConditionalAppearance.Tests.csproj`
- Create: `tests/MyCompany.Module.ConditionalAppearance.EF.Tests/MyCompany.Module.ConditionalAppearance.EF.Tests.csproj`

**Step 1: Create the projects**

Run:
```bash
dotnet new classlib -n MyCompany.Module.ConditionalAppearance -o src/MyCompany.Module.ConditionalAppearance
dotnet new classlib -n MyCompany.Module.ConditionalAppearance.EF -o src/MyCompany.Module.ConditionalAppearance.EF
dotnet new xunit -n MyCompany.Module.ConditionalAppearance.Tests -o tests/MyCompany.Module.ConditionalAppearance.Tests
dotnet new xunit -n MyCompany.Module.ConditionalAppearance.EF.Tests -o tests/MyCompany.Module.ConditionalAppearance.EF.Tests
```

**Step 2: Add project/package references**

Run:
```bash
dotnet add src/MyCompany.Module.ConditionalAppearance/MyCompany.Module.ConditionalAppearance.csproj package DevExpress.ExpressApp
dotnet add src/MyCompany.Module.ConditionalAppearance/MyCompany.Module.ConditionalAppearance.csproj package DevExpress.ExpressApp.ConditionalAppearance
dotnet add src/MyCompany.Module.ConditionalAppearance.EF/MyCompany.Module.ConditionalAppearance.EF.csproj package DevExpress.ExpressApp.EFCore
dotnet add src/MyCompany.Module.ConditionalAppearance.EF/MyCompany.Module.ConditionalAppearance.EF.csproj package DevExpress.Persistent.BaseImpl.EFCore
dotnet add src/MyCompany.Module.ConditionalAppearance.EF/MyCompany.Module.ConditionalAppearance.EF.csproj reference src/MyCompany.Module.ConditionalAppearance/MyCompany.Module.ConditionalAppearance.csproj
```

### Task 2: Contracts and runtime model

**Files:**
- Create: `src/MyCompany.Module.ConditionalAppearance/Contracts/IAppearanceRuleData.cs`
- Create: `src/MyCompany.Module.ConditionalAppearance/Contracts/IAppearanceRuleProperties.cs`
- Create: `src/MyCompany.Module.ConditionalAppearance/Runtime/RuntimeAppearanceRule.cs`

**Step 1: Write failing tests**

Files:
- `tests/MyCompany.Module.ConditionalAppearance.Tests/Unit/AppearanceRuleValidationTests.cs`

Target:
- Rule with empty type is invalid.
- Rule with empty criteria and method is invalid.
- Disabled rule is filtered.

**Step 2: Implement minimal contracts/models to satisfy tests**

### Task 3: Rule translator

**Files:**
- Create: `src/MyCompany.Module.ConditionalAppearance/Services/AppearanceRuleTranslator.cs`
- Create: `src/MyCompany.Module.ConditionalAppearance/Utilities/AppearanceTypeResolver.cs`
- Create: `src/MyCompany.Module.ConditionalAppearance/Utilities/AppearanceRuleValidation.cs`

**Step 1: Write failing tests**

Files:
- `tests/MyCompany.Module.ConditionalAppearance.Tests/Unit/AppearanceRuleTranslatorTests.cs`

Coverage:
- Valid row -> runtime rule.
- Invalid type/criteria -> rejected with diagnostics.
- Priority ordering respected.

**Step 2: Implement translator and validation**

### Task 4: Cache and orchestration

**Files:**
- Create: `src/MyCompany.Module.ConditionalAppearance/Services/AppearanceRuleCache.cs`
- Create: `src/MyCompany.Module.ConditionalAppearance/Controllers/ConditionalAppearanceCollectorController.cs`
- Create: `src/MyCompany.Module.ConditionalAppearance/Controllers/ConditionalAppearanceCacheController.cs`

**Step 1: Write failing tests**

Files:
- `tests/MyCompany.Module.ConditionalAppearance.Tests/Unit/AppearanceRuleCacheTests.cs`

Coverage:
- Cache key segmentation by type/context.
- Invalidation on update/delete notification.
- Rebuild after clear.

**Step 2: Implement cache and controllers**

### Task 5: EF persistence layer

**Files:**
- Create: `src/MyCompany.Module.ConditionalAppearance.EF/BusinessObjects/AppearanceRuleData.cs`
- Create: `src/MyCompany.Module.ConditionalAppearance.EF/Services/EFAppearanceRuleDataProvider.cs`
- Create: `src/MyCompany.Module.ConditionalAppearance.EF/Module/ConditionalAppearanceEFModule.cs`

**Step 1: Write failing integration tests**

Files:
- `tests/MyCompany.Module.ConditionalAppearance.EF.Tests/Integration/EFAppearanceRuleDataProviderTests.cs`

Coverage:
- Query enabled rules by type/context.
- Sorting by priority.
- Disabled and malformed rows excluded.

**Step 2: Implement EF entity/provider/module registration**

### Task 6: Model/UI defaults and admin UX

**Files:**
- Create: `src/MyCompany.Module.ConditionalAppearance.EF/Model/ModelNodesGeneratorUpdater.cs`
- Create: `src/MyCompany.Module.ConditionalAppearance.EF/Model/ConditionalAppearanceModelDefaults.cs`

**Step 1: Add basic list/detail views metadata and property editors**

Focus:
- Type picker for `DataTypeName`
- Guarded criteria editor
- Target-items helper

### Task 7: End-to-end verification

**Files:**
- Create: `tests/MyCompany.Module.ConditionalAppearance.EF.Tests/Integration/RuleApplicationFlowTests.cs`
- Create: `docs/conditional-appearance-ef-only.md`

**Step 1: Test full flow**

Coverage:
- Admin creates rule -> UI changes in target view.
- Admin edits rule -> cache invalidates -> new behavior visible.
- Rule delete disables behavior.

**Step 2: Document supported semantics and known limits**

## Recommended delivery phases

1. Phase A (week 1): Tasks 1-3 (translator pipeline complete).
2. Phase B (week 2): Tasks 4-5 (cache + EF provider + module registration).
3. Phase C (week 3+): Tasks 6-7 (UX polish + E2E + docs).

## Acceptance criteria

1. Rules are editable through an EF-backed BO.
2. Runtime appearance changes apply without redeploy.
3. Cache invalidates correctly after data changes.
4. Invalid rules do not crash UI and are observable in logs.
5. Unit and integration tests pass in CI.

## Primary file names (quick list)

- `ConditionalAppearanceModuleBase.cs`
- `ConditionalAppearanceCollectorController.cs`
- `AppearanceRuleCache.cs`
- `AppearanceRuleTranslator.cs`
- `IAppearanceRuleDataProvider.cs`
- `IAppearanceRuleData.cs`
- `AppearanceRuleData.cs` (EF BO)
- `EFAppearanceRuleDataProvider.cs`
- `ConditionalAppearanceEFModule.cs`
- `ConditionalAppearanceEFModuleBuilderExtensions.cs`
- `ModelNodesGeneratorUpdater.cs`
- `AppearanceRuleTranslatorTests.cs`
- `AppearanceRuleCacheTests.cs`
- `EFAppearanceRuleDataProviderTests.cs`
- `RuleApplicationFlowTests.cs`
