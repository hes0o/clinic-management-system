# 🎨 Merve - UI/Styles Engineer
## مروة - مهندسة الواجهات والتنسيق

---

## 📋 Role Overview | نظرة عامة على الدور

**English:** You are responsible for all visual styling, colors, fonts, and icons. You ensure the app looks professional and consistent across all screens.

**Arabic:** أنت مسؤولة عن جميع التنسيقات المرئية والألوان والخطوط والأيقونات. تضمنين أن التطبيق يبدو احترافياً ومتناسقاً في جميع الشاشات.

---

## 🌿 Branch Rules | قواعد الفروع

| Rule | Description |
|------|-------------|
| **Your Branch** | `feature/styles` |
| **Work Directory** | `/Styles` folder + XAML styles only |
| **Merge To** | `develop` (via PR) |
| **Requires** | Hassan's approval |

### Branch Setup Commands:
```bash
# First time setup
git checkout -b feature/styles
git push -u origin feature/styles

# Daily workflow
git checkout feature/styles
git pull origin develop
# ... do your work ...
git add .
git commit -m "style: your message here"
git push origin feature/styles
```

---

## ✅ Task 1: Create Global Styles File
### المهمة 1: إنشاء ملف التنسيقات العامة

**Priority:** 🔴 High | **Estimated Time:** 2 hours
**File:** `/Styles/GlobalStyles.axaml`

#### English Instructions:
1. Create `/Styles/GlobalStyles.axaml`
2. Define reusable styles for:
   - Buttons (Primary, Secondary, Success, Danger)
   - TextBoxes
   - Cards
   - Headers
3. Use the color palette defined below
4. Register styles in `App.axaml`

#### التعليمات بالعربية:
1. أنشئي ملف `/Styles/GlobalStyles.axaml`
2. عرّفي أنماط قابلة لإعادة الاستخدام
3. استخدمي لوحة الألوان المحددة أدناه
4. سجلي الأنماط في `App.axaml`

#### Color Palette | لوحة الألوان:
```
Primary Blue:    #3B82F6 (buttons, links)
Success Green:   #22C55E (save, confirm)
Danger Red:      #EF4444 (delete, cancel)
Warning Yellow:  #F59E0B (alerts)

Background:      #F1F5F9 (main area)
Sidebar:         #1E293B (dark sidebar)
Card:            #FFFFFF (white cards)

Text Primary:    #1E293B (main text)
Text Secondary:  #64748B (labels)
Text Muted:      #94A3B8 (hints)
```

#### Code Example:
```xml
<!-- /Styles/GlobalStyles.axaml -->
<Styles xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    
    <!-- Primary Button -->
    <Style Selector="Button.btn-primary">
        <Setter Property="Background" Value="#3B82F6"/>
        <Setter Property="Foreground" Value="White"/>
        <Setter Property="CornerRadius" Value="8"/>
        <Setter Property="Padding" Value="16,12"/>
        <Setter Property="FontWeight" Value="SemiBold"/>
        <Setter Property="Cursor" Value="Hand"/>
    </Style>
    <Style Selector="Button.btn-primary:pointerover /template/ ContentPresenter">
        <Setter Property="Background" Value="#2563EB"/>
    </Style>
    
    <!-- Success Button -->
    <Style Selector="Button.btn-success">
        <Setter Property="Background" Value="#22C55E"/>
        <Setter Property="Foreground" Value="White"/>
        <Setter Property="CornerRadius" Value="8"/>
        <Setter Property="Padding" Value="16,12"/>
    </Style>
    
    <!-- Card Style -->
    <Style Selector="Border.card">
        <Setter Property="Background" Value="White"/>
        <Setter Property="CornerRadius" Value="16"/>
        <Setter Property="Padding" Value="24"/>
        <Setter Property="BoxShadow" Value="0 4 6 -1 #1a000000"/>
    </Style>
    
</Styles>
```

---

## ✅ Task 2: Improve Arabic Font Support
### المهمة 2: تحسين دعم الخط العربي

**Priority:** 🔴 High | **Estimated Time:** 1.5 hours

#### English Instructions:
1. Find a good Arabic font (Cairo, Tajawal, or IBM Plex Arabic)
2. Add font files to `/Assets/Fonts/`
3. Register fonts in `App.axaml`
4. Apply to all TextBlocks

#### التعليمات بالعربية:
1. ابحثي عن خط عربي جيد
2. أضيفي ملفات الخط إلى `/Assets/Fonts/`
3. سجلي الخطوط في `App.axaml`
4. طبقيها على جميع النصوص

#### Code Example:
```xml
<!-- In App.axaml -->
<Application.Styles>
    <FluentTheme />
    <StyleInclude Source="/Styles/GlobalStyles.axaml"/>
</Application.Styles>

<Application.Resources>
    <FontFamily x:Key="ArabicFont">avares://HealthCenter.Desktop/Assets/Fonts/Cairo-Regular.ttf#Cairo</FontFamily>
</Application.Resources>
```

---

## ✅ Task 3: Add Loading States & Animations
### المهمة 3: إضافة حالات التحميل والحركات

**Priority:** 🟡 Medium | **Estimated Time:** 2 hours

#### English Instructions:
1. Create a loading spinner component
2. Add hover animations to buttons
3. Add transition effects when navigating
4. Create a "no data" placeholder design

#### التعليمات بالعربية:
1. أنشئي مكون دوار التحميل
2. أضيفي حركات عند التمرير على الأزرار
3. أضيفي تأثيرات انتقالية عند التنقل
4. صممي شكل "لا توجد بيانات"

---

## 📁 Your Files | ملفاتك

```
/Styles (create this folder)
├── GlobalStyles.axaml      ← Create this
├── ButtonStyles.axaml      ← Create this
├── CardStyles.axaml        ← Create this
└── Animations.axaml        ← Create this

/Assets
└── Fonts/                  ← Add fonts here
    ├── Cairo-Regular.ttf
    └── Cairo-Bold.ttf
```

---

## 🎨 Design Guidelines | إرشادات التصميم

- ✅ Corners: Use `CornerRadius="8"` for buttons, `16` for cards
- ✅ Spacing: Use multiples of 4 (4, 8, 12, 16, 20, 24)
- ✅ Shadows: Light shadows for cards, no shadows for buttons
- ✅ RTL: All text flows Right-to-Left (Arabic)
- ❌ Never use pure black (#000000)
- ❌ Never use pure white text on colored buttons

---

## ⚠️ Important Notes | ملاحظات مهمة

- Commit messages must start with `style:`
- Test all screens after making style changes
- Check both light and dark themes if applicable
- Ensure accessibility (good contrast ratios)

**Questions?** Ask Hassan (Team Lead)
