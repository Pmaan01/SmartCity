# SmartCity - Multi-Platform City Information Application

A modern .NET MAUI application that aggregates real-time city information including weather, air quality, news, and parking information in one seamless interface.

## 📋 Table of Contents

- [Features](#features)
- [Screenshots](#screenshots)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [API Configuration](#api-configuration)
- [Building & Deployment](#building--deployment)
- [Architecture](#architecture)
- [Usage Guide](#usage-guide)
- [Contributing](#contributing)

---

## ✨ Features

### 🌤️ Weather Module
- Real-time weather data for any city
- Dynamic background colors based on weather conditions
- 3-day weather forecast
- City search with suggestions
- Temperature display in Celsius
- Last updated timestamp

### 💨 Air Quality Module
- Real-time Air Quality Index (AQI)
- Pollutant information
- Hourly AQI trends
- AQI category classification
- Automatic city synchronization

### 📰 News Module
- Latest news articles for selected city
- Article images and descriptions
- Source and publication date
- Open articles in browser
- Real-time news feed

### 🅿️ Parking Module
- Nearby parking locations
- Parking hours and rates
- Special notes and features
- Get directions integration
- Available spots information

### 🌍 Cross-Platform Features
- **Windows Desktop**: Full UI support
- **Android**: Complete functionality with emulator and device support
- **Responsive Design**: Adapts to all screen sizes
- **City Synchronization**: Select a city once, all pages update automatically

---

## 📸 Screenshots

### Weather Page
- Dynamic gradient background based on conditions
- Large temperature display
- 3-day forecast cards
- City selection picker
- <img width="502" height="1008" alt="1" src="https://github.com/user-attachments/assets/ad6a16a7-b9ff-407e-9745-0c5696f66a69" />
<img width="1396" height="712" alt="5" src="https://github.com/user-attachments/assets/d4e6a3aa-143b-4271-81f6-f142a8a00c8e" />

### Air Quality Page
- Main AQI value display
- Category badge
- Hourly trend visualization
- Air quality reference guide
<img width="492" height="1017" alt="2" src="https://github.com/user-attachments/assets/784dc9e5-8ad0-44c6-aed0-d2c9b8a5f957" />
<img width="1916" height="1108" alt="6" src="https://github.com/user-attachments/assets/fc3289b6-d7ec-4110-be32-a1be1317c1fc" />

### News Page
- Article cards with images
- Source and publication info
- Read full article functionality
- City-specific news filtering
- <img width="1903" height="1025" alt="7" src="https://github.com/user-attachments/assets/7cf1f68a-6436-45fe-ab4a-3778c79c0ef1" />
<img width="497" height="1043" alt="3" src="https://github.com/user-attachments/assets/06385f69-be39-47b8-9ee4-2b0ca1630ee1" />

### Parking Page
- Parking spot details
- Address and operating hours
- Price information
- Special deals/notes
- Get directions button
<img width="520" height="1027" alt="4" src="https://github.com/user-attachments/assets/21a84e77-8486-474a-ab56-a53a2cfab88c" />
<img width="1893" height="1026" alt="8" src="https://github.com/user-attachments/assets/2c4236fa-4f66-4d86-93e0-bc8e415c3ea9" />

---

## 🛠️ Technology Stack

### Framework & Platform
- **.NET MAUI 9.0** - Cross-platform UI framework
- **C# 12** - Programming language
- **Windows 11+** - Desktop platform
- **Android 15.0 (API 35)** - Mobile platform

### Architecture & Patterns
- **MVVM Pattern** - Clean separation of concerns
- **Dependency Injection** - Loose coupling
- **Command Pattern** - User interactions
- **Observer Pattern** - Data binding

### External Services
- **OpenWeatherMap API** - Weather data
- **OpenAQ API** - Air quality data
- **NewsAPI** - News articles
- **Google Maps API** - Parking locations

### Libraries & NuGet Packages
- `CommunityToolkit.Maui` - Extended MAUI controls
- `CommunityToolkit.Mvvm` - MVVM utilities
- `System.Text.Json` - JSON parsing
- `System.Net.Http` - HTTP requests

---

## 📁 Project Structure

```
SmartCity/
├── Models/
│   ├── WeatherModel.cs              # Weather data structure
│   ├── AQModel.cs                   # Air quality data
│   ├── NewsArticle.cs               # News article model
│   └── ParkingSpot.cs               # Parking location model
│
├── Services/
│   ├── IWeatherService.cs           # Weather service interface
│   ├── WeatherService.cs            # OpenWeatherMap integration
│   ├── IAQService.cs                # Air quality interface
│   ├── AQService.cs                 # OpenAQ integration
│   ├── INewsService.cs              # News service interface
│   ├── NewsService.cs               # NewsAPI integration
│   ├── IParkingService.cs           # Parking service interface
│   ├── ParkingService.cs            # Google Maps integration
│   ├── CityStateManager.cs          # Cross-page state management
│   └── CacheService.cs              # Local data caching
│
├── ViewModels/
│   ├── BaseViewModel.cs             # Base view model class
│   ├── WeatherViewModel.cs          # Weather page logic
│   ├── AQViewModel.cs               # Air quality page logic
│   ├── NewsViewModel.cs             # News page logic
│   └── ParkingViewModel.cs          # Parking page logic
│
├── Views/
│   ├── WeatherPage.xaml             # Weather UI
│   ├── WeatherPage.xaml.cs          # Weather code-behind
│   ├── AQPage.xaml                  # Air quality UI
│   ├── AQPage.xaml.cs               # Air quality code-behind
│   ├── NewsPage.xaml                # News UI
│   ├── NewsPage.xaml.cs             # News code-behind
│   ├── ParkingPage.xaml             # Parking UI
│   └── ParkingPage.xaml.cs          # Parking code-behind
│
├── Converters/
│   ├── HasValueConverter.cs         # String to bool converter
│   └── IsGreaterThanZeroConverter.cs # Numeric value converter
│
├── Resources/
│   └── Styles/
│       ├── Colors.xaml              # Color definitions
│       └── Styles.xaml              # Style definitions
│
├── App.xaml                          # Application shell
├── App.xaml.cs                       # Application code-behind
├── MauiProgram.cs                    # Dependency injection setup
├── AppShell.xaml                     # Navigation shell
├── appsettings.json                  # API keys configuration
└── README.md                         # This file
```

---

## 🚀 Getting Started

### Prerequisites
- Visual Studio 2022 or newer
- .NET 9.0 SDK installed
- Android SDK (for Android development)
- API keys from:
  - OpenWeatherMap (https://openweathermap.org/api)
  - OpenAQ (https://openaq.org/)
  - NewsAPI (https://newsapi.org/)
  - Google Maps (https://developers.google.com/maps)

### Installation

1. **Clone or download the project**
```bash
git clone https://github.com/yourusername/SmartCity.git
cd SmartCity
```

2. **Install dependencies**
```bash
dotnet restore
```

3. **Configure API keys** (see API Configuration section)

4. **Open in Visual Studio**
```bash
devenv SmartCity.sln
```

---

## 🔑 API Configuration

### Method 1: Using appsettings.json (Recommended)

1. Create `appsettings.json` in project root:
```json
{
  "ApiKeys": {
    "OpenWeatherMap": "your_api_key_here",
    "OpenAQ": "your_api_key_here",
    "NewsAPI": "your_api_key_here",
    "GoogleMaps": "your_api_key_here"
  }
}
```

2. Set file properties:
   - Right-click `appsettings.json`
   - **Properties** → **Copy to Output Directory** → **Copy Always**

### Method 2: Using launchSettings.json

Edit `Properties/launchSettings.json`:
```json
{
  "profiles": {
    "Windows Machine": {
      "environmentVariables": {
        "OPENWEATHER_API_KEY": "your_key",
        "OPENAQ_API_KEY": "your_key",
        "NEWSAPI_API_KEY": "your_key",
        "GOOGLE_MAPS_API_KEY": "your_key"
      }
    },
    "Android Emulator": {
      "environmentVariables": {
        "OPENWEATHER_API_KEY": "your_key",
        "OPENAQ_API_KEY": "your_key",
        "NEWSAPI_API_KEY": "your_key",
        "GOOGLE_MAPS_API_KEY": "your_key"
      }
    }
  }
}
```

### Getting API Keys

**OpenWeatherMap**: https://openweathermap.org/api
- Free tier includes current weather and 5-day forecast
- No credit card required

**OpenAQ**: https://openaq.org/
- Free API for air quality data
- No authentication needed

**NewsAPI**: https://newsapi.org/
- Free tier: 100 requests/day
- Sign up for API key

**Google Maps**: https://developers.google.com/maps
- Enable Places API and Maps JavaScript API
- Create API key from GCP Console

---

## 🏗️ Building & Deployment

### Run on Windows
1. Select **"Windows Machine"** from dropdown
2. Press **F5** or click **Run**
3. App launches in Windows window

### Run on Android Emulator
1. Start Android Emulator (Pixel 7 - API 35)
2. Select **"Android Emulator"** from dropdown
3. Press **F5** or click **Run**
4. App builds and deploys to emulator

### Run on Physical Android Device
1. Connect device via USB
2. Enable USB Debugging on device
3. Select **"Android Device"** from dropdown
4. Press **F5**

### Build for Release
```bash
dotnet publish -f net9.0-windows -c Release
dotnet publish -f net9.0-android -c Release
```

---

## 🏛️ Architecture

### Architectural Patterns

**MVVM (Model-View-ViewModel)**
```
View (XAML)
    ↓ (Data Binding)
ViewModel (C#)
    ↓ (Services)
Models + Services
    ↓ (HTTP)
External APIs
```

### Data Flow

```
User selects city on Weather Page
    ↓
CityStateManager.SelectedCity = city
    ↓
All other ViewModels subscribe to changes
    ↓
Services fetch data for new city
    ↓
Models updated
    ↓
UI automatically refreshes via binding
```

### State Management

**CityStateManager** (Singleton)
- Holds current selected city
- Notifies all subscribers of city changes
- Ensures all pages stay synchronized
- Implements INotifyPropertyChanged

---

## 📖 Usage Guide

### Selecting a City

1. **Weather Page** → City Picker or Search Box
2. Type city name or select from dropdown
3. Click search button or press Enter
4. Other pages automatically update

### Viewing Weather
1. Open Weather page
2. Current temperature and conditions display
3. Scroll down for 3-day forecast
4. Background color changes based on weather

### Checking Air Quality
1. Open Air Quality page
2. View current AQI score
3. Check category (Good, Moderate, Unhealthy, etc.)
4. Scroll for hourly trend data

### Reading News
1. Open News page
2. Browse articles for selected city
3. Click "Read Full Article" to open in browser
4. Articles auto-load when city changes

### Finding Parking
1. Open Parking page
2. View nearby parking locations
3. Check hours, rates, and special features
4. Click "Get Directions" to navigate

---

## 🤝 Contributing

### Code Style
- Use PascalCase for classes and methods
- Use camelCase for variables and parameters
- Use meaningful names
- Add XML documentation to public methods
- Keep methods small and focused

### Making Changes
1. Create a new branch for features
2. Make changes with clear commit messages
3. Test on Windows and Android
4. Submit pull request with description

### Testing Checklist
- [ ] App runs on Windows
- [ ] App runs on Android Emulator
- [ ] City selection works
- [ ] All pages update when city changes
- [ ] API data loads correctly
- [ ] Mock data shows when offline
- [ ] No crashes or exceptions
- [ ] UI is responsive on all screen sizes

---

## 🐛 Troubleshooting

### API Keys Not Loading
- Check `appsettings.json` file exists in project root
- Verify "Copy Always" is set in file properties
- Clean and rebuild solution

### Android Deployment Issues
- Ensure emulator is running fully
- Run `adb kill-server` then `adb start-server`
- Delete `bin/` and `obj/` folders
- Rebuild solution

### No Data Displaying
- Check internet connection
- Verify API keys are valid
- Check API rate limits not exceeded
- App will show mock data if API fails

### City Not Syncing Across Pages
- Restart the application
- Select a city from dropdown (not search)
- Check console for error messages

---

## 📝 License

This project is created for educational purposes as part of .NET MAUI course assignment.

---

## 👤 Author

Created as an individual project for .NET MAUI Development Course

---

## 🙏 Acknowledgments

- Microsoft MAUI Documentation: https://learn.microsoft.com/en-us/dotnet/maui/
- OpenWeatherMap API: https://openweathermap.org/
- OpenAQ: https://openaq.org/
- NewsAPI: https://newsapi.org/
- Google Maps API: https://developers.google.com/maps
- Microsoft MAUI Samples: https://github.com/microsoft/maui-samples

---

## 📞 Support

For issues or questions:
1. Check the Troubleshooting section above
2. Review the API documentation
3. Check console output for error messages
4. Verify all dependencies are installed
