# TODO

- Decouple Services, Views and ViewModels completely
- Make popup windows proper "dialogs".
- Migrate ViewModel DTOs into View-formatted displays and proper DTOs
    - Make proper to and from DTO methods for DTO-Model conversion.
- Systematically extract and make frequent or complex queries part of IQueryable extension family

## Concurrency / Background Thread Safety

- `SafeFireAndForget` uses `ConfigureAwait(true)`. If any fire-and-forget is ever invoked from a background thread, continuations will lack the Dispatcher and UI property updates will throw.
- `TiepNhanPhieuGoiMonViewModel` modifies `OrderDetails` (ObservableCollection) from command handlers with no explicit thread-affinity assertion. May break if background access is ever introduced.
- `UpdateAllowedDonViTinhs` was made synchronous (pre-loaded lookup data, in-memory filter). If the lookup table grows or network latency is introduced (e.g., remote DB), it may need to revert to async with proper cancellation. Changed to avoid fire-and-forget unsafety. Potential user annoyance if LoaiMonAn or DVT or LoaiMonAn-DVT is updated in the background while the popup is open.

## Navigation

- Refactor `MainViewModel` navigation state — the growing list of `Is[Page]Active` boolean flags and `NavigateTo[Page]` commands does not scale. Consider a navigation model with a single `ActivePage` enum and a shared `Navigate` command, or a dictionary-based active-state lookup.

## TraCuuBanAnViewModel - Note

```cs
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.LoaiBan
                .AsNoTracking()
                .OrderBy(l => l.PhuThu)
                .Select(l => new LoaiBanOption(l.MaLoaiBan, l.TenLoaiBan))
                .ToListAsync();
```
