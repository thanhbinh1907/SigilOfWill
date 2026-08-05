# Nhật ký sửa lỗi Crash - Sigil Of Will

Tài liệu này lưu trữ các lỗi crash hệ thống (native crash/runtime crash), tình trạng xử lý và biện pháp phòng ngừa cho dự án game **Sigil of Will**.

---

## 1. ĐÃ SỬ A (RESOLVED)

### Lỗi crash khi ngồi / đứng dậy tại Trạm Nghỉ (Site of Grace)
* **Triệu chứng:** Game tự động đóng lập tức (native crash) ngay khi người chơi tương tác với lửa trại (Site of Grace) để ngồi xuống hoặc đứng lên.
* **Nguyên nhân:** Lỗi liên kết Script trong các tệp Animator Controller (`Humanoid.controller`, `DarkLord.controller`, `Frost Giant.controller`, và các bộ điều khiển Undead). Các bộ điều khiển này chứa tham chiếu đến các State Machine Behaviour bị thiếu hoặc định dạng sai lớp lớp editor (sử dụng dấu `::` dạng `::ResetIsJumping` thay vì định dạng hợp lệ `Assembly-CSharp::SG.ResetIsJumping`).
* **Cách khắc phục:**
  1. Tạo lại chính xác hai mã nguồn Behavior: [ResetIsJumping.cs](file:///c:/Binh/Nam%204/Ki%202/DoAn/SigilOfWill/SigilOfWill/Assets/_Project/Scripts/Gameplay/Character/Animators/ResetIsJumping.cs) và [ResetActionFlag.cs](file:///c:/Binh/Nam%204/Ki%202/DoAn/SigilOfWill/SigilOfWill/Assets/_Project/Scripts/Gameplay/Character/Animators/ResetActionFlag.cs) đặt dưới namespace `SG`.
  2. Viết tập lệnh thay thế tự động trong các tệp `.controller` để chuyển đổi các thuộc tính `m_Script` bị hỏng thành cấu trúc định danh GUID và FileID hợp lệ trỏ tới `Assembly-CSharp::SG.ResetIsJumping` và `Assembly-CSharp::SG.ResetActionFlag`.
  3. Thêm cơ chế kiểm tra trạng thái tương tác trong [PlayerInputManager.cs](file:///c:/Binh/Nam%204/Ki%202/DoAn/SigilOfWill/SigilOfWill/Assets/_Project/Scripts/Gameplay/Character/Player/PlayerInputManager.cs) để từ chối nhận thêm tương tác mới khi người chơi đang trong hoạt ảnh ngồi/đứng dậy tại Grace.

### Lỗi crash khi Nhặt Vật phẩm (Item Pickup)
* **Triệu chứng:** Người chơi đến gần và bấm `E` để nhặt đồ (ví dụ: gậy phép `Sky Staff` ID 5), log ghi nhận người chơi đã nhặt thành công, nhưng game lập tức crash native và thoát ra màn hình.
* **Nguyên nhân:** Lớp [PlayerUISelectButtonOnEnable.cs](file:///c:/Binh/Nam%204/Ki%202/DoAn/SigilOfWill/SigilOfWill/Assets/_Project/Scripts/Gameplay/UI/PlayerUISelectButtonOnEnable.cs) được gắn vào nút bấm trên Panel Popup nhận vật phẩm. Trong hàm `OnEnable()`, mã nguồn gọi:
  ```csharp
  button.Select();
  button.OnSelect(null);
  ```
  Truyền tham số `null` vào hàm `OnSelect` của Unity UI là không hợp lệ. Trong phiên bản Unity 6 (6000.3.14f1) chạy build Standalone, điều này dẫn đến việc EventSystem cố gắng truy cập dữ liệu sự kiện rỗng, gây lỗi con trỏ null (null pointer dereference) trực tiếp trong nhân engine C++ gây crash game ngay lập tức.
* **Cách khắc phục:**
  1. Loại bỏ dòng gọi trực tiếp `button.OnSelect(null)` không an toàn.
  2. Sử dụng hàm Coroutine có sẵn `SelectButtonDelayed()` để trì hoãn 1 frame (chờ UI và EventSystem khởi tạo hoàn chỉnh) rồi gọi `EventSystem.current.SetSelectedGameObject(button.gameObject)` một cách an toàn.

### Lỗi crash khi đứng dậy từ Grace (NavMeshAgent Off-Mesh Crash)
* **Triệu chứng:** Người chơi đến ngồi nghỉ chân tại Trạm Grace, khi đứng dậy game lập tức crash native. Hoặc khi chạy được một lúc thì đột ngột crash (trong file log ghi nhận quái vật spawn lại và in ra các dòng `BLOCKED` do phát hiện người chơi bị chặn tầm nhìn, sau đó bắt đầu truy đuổi thì crash lập tức).
* **Nguyên nhân:**
  1. Khi ngồi tại Grace, game kích hoạt dọn dẹp và hồi sinh lại toàn bộ quái vật thông thường bằng `AICharacterSpawner`. Tuy nhiên, nếu các Spawner được đặt hơi lệch so với bề mặt lưới NavMesh, biến `navMeshAgent.isOnNavMesh` sẽ là `false`. Một `NavMeshAgent` hoạt động ngoài lưới có thể gây lỗi truy cập bộ nhớ trực tiếp trong nhân C++ PhysX/NavMesh của Unity gây crash game.
  2. Các hàm kiểm tra cũ trong trạng thái di chuyển (`PursueTargetState` hoặc `CombatStanceState`) khi thấy Agent bị tắt (`enabled = false`) sẽ tự động bật nó lên lại (`enabled = true`). Tuy nhiên, do Agent lúc này đang không nằm trên NavMesh (và logic warp cũ không chạy được vì Agent đang bị disable), việc bật lại Agent này lập tức kích hoạt lỗi crash native.
* **Cách khắc phục:**
  1. Tại hàm sinh quái vật trong [AICharacterSpawner.cs](file:///c:/Binh/Nam%204/Ki%202/DoAn/SigilOfWill/SigilOfWill/Assets/_Project/Scripts/Gameplay/Character/AI/AICharacterSpawner.cs), tăng tầm tìm kiếm điểm snap NavMesh lên `10.0f`. Nếu không tìm thấy bất cứ điểm NavMesh nào trong bán kính 10m, bắt buộc phải thiết lập `agent.enabled = false` để loại bỏ Agent khỏi luồng cập nhật PhysX native.
  2. Trong [AICharacterManager.cs](file:///c:/Binh/Nam%204/Ki%202/DoAn/SigilOfWill/SigilOfWill/Assets/_Project/Scripts/Gameplay/Character/AI/AICharacterManager.cs) tại hàm `ProcessStateMachine()`, bổ sung kiểm tra: nếu `navMeshAgent.enabled` là `true` nhưng `navMeshAgent.isOnNavMesh` là `false`, chủ động tắt Agent đi ngay lập tức.
  3. Sửa lại hoàn toàn logic phục hồi và kích hoạt Agent trong [PursueTargetState.cs](file:///c:/Binh/Nam%204/Ki%202/DoAn/SigilOfWill/SigilOfWill/Assets/_Project/Scripts/Gameplay/Character/AI/States/PursueTargetState.cs) và [CombatStanceState.cs](file:///c:/Binh/Nam%204/Ki%202/DoAn/SigilOfWill/SigilOfWill/Assets/_Project/Scripts/Gameplay/Character/AI/States/CombatStanceState.cs). Nếu phát hiện Agent bị tắt hoặc không nằm trên NavMesh, thực hiện tìm kiếm điểm NavMesh gần nhất trong vòng 10m, dịch chuyển (teleport) transform của nhân vật về điểm đó khi Agent đang tắt, sau đó mới kích hoạt lại Agent (`enabled = true`). Nếu không thể tìm thấy điểm snap hợp lệ, giữ nguyên trạng thái tắt (`enabled = false`) của Agent để ngăn chặn crash game.

### Lỗi crash khi trang bị vũ khí (Weapon Equip NullReference Crash)
* **Triệu chứng:** Người chơi mở túi đồ, chọn trang bị gậy phép `Sky Staff` (hoặc vũ khí khác) vào ô vũ khí tay phải/tay trái, ngay sau khi nạp thì game bị crash lập tức.
* **Nguyên nhân:**
  1. Khi nạp vũ khí trong [PlayerEquipmentManager.cs](file:///c:/Binh/Nam%204/Ki%202/DoAn/SigilOfWill/SigilOfWill/Assets/_Project/Scripts/Gameplay/Character/Player/PlayerEquipmentManager.cs), game thực hiện Instantiate model vũ khí `Instantiate(weapon.weaponModel)`. Nếu model vũ khí hoặc một số vũ khí đặc biệt (như Unarmed hoặc Staff chưa gán Model) bị rỗng (`null`), gọi Instantiate sẽ dẫn đến biệt lệ native crash.
  2. Đoạn mã gọi `PlayerUIManager.instance.playerUIHudManager.SetRightWeaponQuickSlotIcon(...)` trực tiếp mà không kiểm tra xem `playerUIHudManager` có bị `null` hay không (ví dụ khi HUD bị ẩn ở Main Menu hoặc chưa kịp khởi tạo xong), dẫn đến lỗi NullReferenceException và gây crash trên bản build Standalone.
  3. Lớp [PlayerUIEquipmentManager.cs](file:///c:/Binh/Nam%204/Ki%202/DoAn/SigilOfWill/SigilOfWill/Assets/_Project/Scripts/Gameplay/UI/PlayerUIEquipmentManager.cs) truy xuất trực tiếp các Image của các ô tay phải/tay trái mà không kiểm tra null, nếu trong prefab không gán đủ các slot này sẽ gây crash.
* **Cách khắc phục:**
  1. Thêm kiểm tra an toàn `weaponModel != null` trước khi Instantiate model vũ khí trong cả hai hàm `LoadRightWeapon()` và `LoadLeftWeapon()`.
  2. Thêm kiểm tra điều kiện `PlayerUIManager.instance.playerUIHudManager != null` trước khi gọi cập nhật Icon vũ khí nhanh trên thanh HUD.
  3. Thêm các kiểm tra null toàn diện cho toàn bộ các Image ô trang bị vũ khí (`rightHandSlot01`, `leftHandSlot01`,...) trong `PlayerUIEquipmentManager.cs` trước khi bật tắt hoặc gán sprite hình ảnh.

---

## 2. ĐANG SỬA (IN-PROGRESS)
* Không có lỗi nào đang trong quá trình xử lý.

---

## 3. SẼ SỬ A TRONG TƯƠNG LAI & PHÒNG NGỪA (FUTURE PREVENTIONS)

### An toàn đa luồng khi tự động lưu game (Save Game Thread-Safety)
* **Nguy cơ tiềm ẩn:** Trong [SaveFileDataWriter.cs](file:///c:/Binh/Nam%204/Ki%202/DoAn/SigilOfWill/SigilOfWill/Assets/_Project/Scripts/Core/GameSaving/SaveFileDataWriter.cs), việc lưu game được thực hiện bất đồng bộ bằng luồng phụ (`System.Threading.Tasks.Task.Run`). Tuy nhiên, nếu có lỗi xảy ra ghi đè tệp tin, khối `catch` gọi trực tiếp `Debug.LogError` của Unity. Trên một số nền tảng hoặc phiên bản Unity, gọi các hàm API đồ họa hoặc Debug từ luồng phụ có thể gây xung đột luồng dẫn đến crash hoặc deadlock.
* **Biện pháp phòng ngừa:** Thay thế lời gọi `Debug.LogError` bằng cơ chế ghi log độc lập luồng như `System.Console.WriteLine` hoặc lưu thông tin lỗi và đẩy về luồng chính xử lý qua Dispatcher/Coroutine (Đã áp dụng chuyển sang Console.WriteLine).

### Hiện HUD khi ở màn hình chính (Title Screen HUD Bug)
* **Nguy cơ tiềm ẩn:** Khi build bản release, giao diện HUD của người chơi (Player HUD) hiển thị ngay tại Main Menu mặc dù chưa vào thế giới game. Điều này có thể dẫn đến việc các script HUD cố gắng tham chiếu tới PlayerManager chưa được khởi tạo, gây NullReferenceException hoặc tệ hơn là crash nếu các thành phần đồ họa gọi cập nhật chỉ số liên tục.
* **Biện pháp phòng ngừa:** Đảm bảo toàn bộ hệ thống Player HUD chỉ kích hoạt hiển thị khi scene thế giới thực tế được tải xong (thông qua Event kiểm tra SceneIndex hoặc trạng thái `isSceneLoading` của `WorldSaveGameManager`).
