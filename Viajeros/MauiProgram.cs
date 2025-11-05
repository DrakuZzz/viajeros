using Microsoft.Extensions.Logging;
using Viajeros.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Viajeros
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
            builder.Logging.SetMinimumLevel(LogLevel.Debug);
#endif

            builder.Services.AddSingleton(sp =>
            {
                var handler = new HttpClientHandler();
#if DEBUG
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif
                return new HttpClient(handler)
                {
                    Timeout = TimeSpan.FromSeconds(30)
                };
            });

            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<ImageUploadService>();

#if ANDROID
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler(typeof(Microsoft.AspNetCore.Components.WebView.Maui.BlazorWebView), typeof(CustomBlazorWebViewHandler));
            });
#endif

            Console.WriteLine("✅ Servicios registrados correctamente");
            return builder.Build();
        }
    }

#if ANDROID
    public class CustomBlazorWebViewHandler : Microsoft.AspNetCore.Components.WebView.Maui.BlazorWebViewHandler
    {
        protected override void ConnectHandler(Android.Webkit.WebView platformView)
        {
            base.ConnectHandler(platformView);

            // ✅ CRÍTICO: Permitir contenido mixto (HTTP en página HTTPS)
            platformView.Settings.MixedContentMode = Android.Webkit.MixedContentHandling.AlwaysAllow;
            platformView.Settings.BlockNetworkImage = false;
            platformView.Settings.LoadsImagesAutomatically = true;
            platformView.Settings.CacheMode = Android.Webkit.CacheModes.NoCache;

            // ✅ NUEVO: Deshabilitar seguridad adicional
            platformView.Settings.AllowFileAccessFromFileURLs = true;
            platformView.Settings.AllowUniversalAccessFromFileURLs = true;

            Console.WriteLine("✅ WebView configurado para permitir contenido mixto HTTP/HTTPS");
        }
    }
#endif
}