# TODO

- Decouple Services, Views and ViewModels completely
- Make popup windows "dialog" instead.
- Migrate ViewModel DTOs into View-formatted displays
- Check `_phuThuVal` in `TiepNhanBanAnViewModel`
- Eliminate transient 'LoaiBan' entity models featuring fabricated primary keys ("All") to populate presentation layer dropdown selections.
    1. Extract a presentation DTO or record type: 'public record LoaiBanOption(string Id, string DisplayName);'
    2. Change the view model state variable '_loaiBans' from 'ObservableCollection<LoaiBan>' to 'ObservableCollection<LoaiBanOption>'.
    3. Update 'InitializeFormAsync' to project the database records directly into this option type using a clean LINQ '.Select()' block, and prepend the UI-specific sentinel option securely using '.Prepend(new LoaiBanOption("All", "Tất cả"))'.
    4. Adjust the corresponding ComboBox bindings inside 'TraCuuBanAnWindow.xaml' to target 'SelectedValuePath="Id"' and 'DisplayMemberPath="DisplayName"'.