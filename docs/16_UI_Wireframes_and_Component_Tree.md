# UI Wireframes & Component Tree

## 1. Design Tokens (Tasarım Değişkenleri)
Uygulama TailwindCSS kullanılarak aşağıdaki tokenlar üzerine inşa edilecektir:
* **Primary Color:** `#0F172A` (Slate 900) - Kurumsal ve ciddi görünüm.
* **Accent/Action:** `#2563EB` (Blue 600) - Tıklanabilir butonlar.
* **States:** Success `#16A34A` (Green), Warning `#D97706` (Amber), Danger `#DC2626` (Red).
* **Typography:** `Inter` (Sistem arayüzü), Veri tablolarında `Roboto Mono`.

## 2. Component Tree (Web Dashboard)
Modüler UI bileşen ağacı:
```text
<AppLayout>
  <Sidebar>
     <NavigationMenu />
     <TenantSwitcher />
  </Sidebar>
  <MainContent>
     <TopHeader>
        <GlobalSearch />
        <NotificationBadge />
        <UserProfile />
     </TopHeader>
     <PageContainer>
        <Breadcrumb />
        <!-- Page Specific -->
        <WorkOrderListView>
           <AdvancedFilterBar />
           <DataGrid>
              <ColumnConfigurator />
              <Pagination />
           </DataGrid>
        </WorkOrderListView>
     </PageContainer>
  </MainContent>
</AppLayout>
```

## 3. Mobil Teknisyen Arayüzü (Wireframe Konsepti)
Teknisyenin saha hızını artırmak için tasarlanmış ekran:
* **Üst Bar:** Acil Durum / SOS Butonu. İnternet Durumu İkonu (Offline/Online).
* **Ana Kart:** O anki en acil İş Emri büyük puntoyla gösterilir.
* **Aksiyon Butonları (Devasa Boyut):** [ İŞE BAŞLA ] (Yeşil), [ DURAKLAT ] (Sarı).
* **Sesli Not Butonu:** Ekranın alt yarısını kaplayan basılı tutulabilir [ SESLİ NOT KAYDET ] butonu.
* **Kamera:** Hızlıca fotoğraf çekmek için ekranda her an açık bir kamera shortcut'ı.

## 4. Component Tree (Mobil React Native)
Teknisyenin sahada kullandığı uygulamanın React Native yapısı:
```text
<MobileApp>
  <OfflineSyncProvider>
     <NavigationStack>
        <DashboardScreen>
           <StatusHeader />
           <UrgentWorkOrderCard />
           <QuickActionGrid />
        </DashboardScreen>
        
        <WorkOrderExecutionScreen>
           <SafetyChecklistModal /> <!-- PPE ve LOTO onayları -->
           <TimerWidget />
           <PushToTalkButton /> <!-- Sesli not ve Speech-to-text -->
           <CameraWidget />
           <MaterialConsumptionList />
        </WorkOrderExecutionScreen>
     </NavigationStack>
  </OfflineSyncProvider>
</MobileApp>
```
