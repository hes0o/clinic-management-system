# 👑 Hassan - Team Lead
## حسن - قائد الفريق

---

## 📋 Role Overview | نظرة عامة على الدور

**English:** You are the Team Lead responsible for code reviews, merging branches, resolving conflicts, and ensuring the team follows best practices.

**Arabic:** أنت قائد الفريق المسؤول عن مراجعة الكود ودمج الفروع وحل التعارضات والتأكد من التزام الفريق بأفضل الممارسات.

---

## 🌿 Branch Rules | قواعد الفروع

| Rule | Description |
|------|-------------|
| **Your Branch** | `main` (direct access) |
| **Protected Branches** | `main`, `develop` |
| **Merge Authority** | Only YOU can merge to `main` |
| **Review Required** | All PRs need your approval |

---

## ✅ Task 1: Daily Code Review
### المهمة 1: مراجعة الكود اليومية

**Priority:** 🔴 High | **Estimated Time:** 1-2 hours daily

#### English Instructions:
1. Check GitHub for new Pull Requests every morning
2. Review code changes in each PR:
   - Check for bugs and logic errors
   - Ensure code follows project structure
   - Verify Arabic RTL is maintained in UI
   - Check for hardcoded strings (should be in resources)
3. Leave comments on issues found
4. Approve or request changes
5. Merge approved PRs to `develop` branch

#### التعليمات بالعربية:
1. تحقق من GitHub للطلبات الجديدة كل صباح
2. راجع تغييرات الكود في كل طلب
3. اترك تعليقات على المشاكل الموجودة
4. وافق أو اطلب تغييرات
5. ادمج الطلبات الموافق عليها

#### Commands:
```bash
# Fetch latest changes
git fetch origin

# Review a branch locally
git checkout feature/branch-name
dotnet build
dotnet run

# Merge to develop
git checkout develop
git merge feature/branch-name
git push origin develop
```

---

## ✅ Task 2: Conflict Resolution
### المهمة 2: حل التعارضات

**Priority:** 🔴 High | **Estimated Time:** As needed

#### English Instructions:
1. When merge conflicts occur, team members will notify you
2. Pull both conflicting branches locally
3. Use VS Code or Rider to resolve conflicts
4. Test the resolved code thoroughly
5. Push the resolution

#### التعليمات بالعربية:
1. عند حدوث تعارضات، سيبلغك أعضاء الفريق
2. اسحب الفرعين المتعارضين محلياً
3. استخدم VS Code أو Rider لحل التعارضات
4. اختبر الكود بعد الحل
5. ارفع الحل

#### Commands:
```bash
# If conflict occurs
git merge feature/some-branch
# << CONFLICT APPEARS >>

# After resolving in editor:
git add .
git commit -m "resolve: Merge conflict between X and Y"
git push origin develop
```

---

## ✅ Task 3: Weekly Release to Main
### المهمة 3: الإصدار الأسبوعي

**Priority:** 🟡 Medium | **Estimated Time:** 30 minutes weekly

#### English Instructions:
1. Every Sunday, prepare a release from `develop` to `main`
2. Ensure all tests pass
3. Update version number if applicable
4. Create a release tag
5. Notify team of new release

#### التعليمات بالعربية:
1. كل يوم أحد، جهز إصدار من develop إلى main
2. تأكد من نجاح جميع الاختبارات
3. حدث رقم الإصدار
4. أنشئ tag للإصدار
5. أبلغ الفريق بالإصدار الجديد

#### Commands:
```bash
git checkout main
git merge develop
git tag -a v1.0.0 -m "Release v1.0.0"
git push origin main --tags
```

---

## 📞 Team Contacts | جهات اتصال الفريق

| Member | Role | Branch |
|--------|------|--------|
| Bassam | Database | `feature/database` |
| Merve | Styles | `feature/styles` |
| Wissam | Reception | `feature/reception` |
| Ela | Doctor | `feature/doctor` |
| Eylaf | Queue | `feature/queue` |
| Ahmed | Infrastructure | `feature/infrastructure` |
| Asma | Testing | `feature/testing` |

---

## ⚠️ Important Notes | ملاحظات مهمة

- Never force push to `main` or `develop`
- Always pull before making changes
- Keep the team updated on Slack/WhatsApp
- Document any major decisions

**Questions?** You're the lead, figure it out! 😄
