using MudBlazor;
using MudBlazor.Extensions;

namespace GanPersonWeb.Client.Services
{

    // 主题类
    public class Theme
    {
        public string Name { get; set; } = string.Empty;
        public MudTheme MudTheme { get; set; } = new MudTheme();
        public string ThemeWallpaperUrl { get; set; } = string.Empty;
        public string ThemeBackgroundUrl { get; set; } = string.Empty;
    }

    // 静态预设主题类
    public static class MyThemes
    {

        public static PaletteLight defaultLight = new PaletteLight()
        {
            Black = "#110e2d",
            AppbarText = "#424242",
            AppbarBackground = "rgba(255,255,255,0.8)",
            DrawerBackground = "#ffffff",
            GrayLight = "#e8e8e8",
            GrayLighter = "#f9f9f9",
        };

        public static PaletteDark defaultDark = new PaletteDark()
        {
            Primary = "#7e6fff",
            Surface = "#1e1e2d",
            Background = "#1a1a27",
            BackgroundGray = "#151521",
            AppbarText = "#92929f",
            AppbarBackground = "rgba(26,26,39,0.8)",
            DrawerBackground = "#1a1a27",
            ActionDefault = "#74718e",
            ActionDisabled = "#9999994d",
            ActionDisabledBackground = "#605f6d4d",
            TextPrimary = "#b2b0bf",
            TextSecondary = "#92929f",
            TextDisabled = "#ffffff33",
            DrawerIcon = "#92929f",
            DrawerText = "#92929f",
            GrayLight = "#2a2833",
            GrayLighter = "#1e1e2d",
            Info = "#4a86ff",
            Success = "#3dcb6c",
            Warning = "#ffb545",
            Error = "#ff3f5f",
            LinesDefault = "#33323e",
            TableLines = "#33323e",
            Divider = "#292838",
            OverlayLight = "#1e1e2d80",
        };

        public static PaletteLight noteLight = new PaletteLight()
        {
            Black = "#F0EFE0",
            AppbarText = "#424242",
            AppbarBackground = "#E18B43",
            DrawerBackground = "#E18B43",
            GrayLight = "#e8e8e8",
            GrayLighter = "#f9f9f9",
        };

        public static Theme defaultTheme = new Theme()
        {
            Name = "default",
            MudTheme = new()
            {
                PaletteLight = defaultLight,
                PaletteDark = defaultDark,
                LayoutProperties = new LayoutProperties()
            }
        };

        public static Theme noteTheme = new Theme()
        {
            Name = "note",
            MudTheme = new()
            {
                PaletteLight = noteLight,
                PaletteDark = defaultDark,
                LayoutProperties = new LayoutProperties()
            }
        };
    }

    public class ThemePaletteService
    {
        private List<Theme> mudThemesList = new List<Theme>();

        public ThemePaletteService()
        {
            mudThemesList.Add(MyThemes.defaultTheme);
            mudThemesList.Add(MyThemes.noteTheme);
        }

        public List<Theme> GetThemes()
        {
            return mudThemesList;
        }

        public Theme GetTheme(string name)
        {
            // 使用 FirstOrDefault 查找匹配的主题
            Theme theme = mudThemesList.FirstOrDefault(t => t.Name == name);
            if (theme == null) 
            {
                return MyThemes.defaultTheme;
            }
            return theme;
        }
    }
}
