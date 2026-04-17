# Tài liệu tổng kết thay đổi mã nguồn - CapstoneProjectRegistration

Tài liệu này dùng để tóm tắt những phần đã được bổ sung và chỉnh sửa trong dự án backend `CapstoneProjectRegistration`, nhằm giúp các thành viên trong nhóm nhanh chóng nắm được:
- Dự án hiện đang ở giai đoạn nào
- Đã thêm những phần code gì
- Đã thay đổi logic gì so với phần khung ban đầu
- Luồng nghiệp vụ hiện tại đang hoạt động ra sao
- Cần dùng những dữ liệu nào để demo

## 1. Ngữ cảnh hiện tại của dự án

`CapstoneProjectRegistration` là hệ thống backend phục vụ bài toán quản lý đăng ký đề tài capstone.

Mục tiêu hiện tại của dự án:
- Tập trung vào backend RESTful API bằng `.NET 8`
- Phục vụ demo nghiệp vụ chính, chưa ưu tiên phần login/auth
- Cho phép giảng viên tạo đề tài, admin phân công reviewer, reviewer đánh giá đề tài, hệ thống tổng hợp kết quả và public đề tài nếu hợp lệ

Kiến trúc hiện tại của solution:
- `CapstoneProjectRegistration.API`
- `CapstoneProjectRegistration.Services`
- `CapstoneProjectRegistration.Repositories`

Mô hình phân tầng hiện tại:
- `API`: nhận request và trả response
- `Services`: xử lý business logic
- `Repositories`: quản lý entity, DbContext, migration, seed data và truy cập dữ liệu

## 2. Tình trạng ban đầu trước khi mở rộng

Ban đầu hệ thống mới ở mức khung cơ bản:
- Có cấu trúc 3 tầng
- Có một số entity nền tảng như `Lecturer`, `Student`, `Topic`, `TopicReview`, `Semester`
- Có repository generic và unit of work
- Có một số API và service cơ bản cho `Topic`

Tuy nhiên trước khi được mở rộng, hệ thống còn thiếu:
- Đợt đăng ký riêng (`RegistrationPeriod`)
- Nhiều field nghiệp vụ quan trọng cho `Topic`
- Ràng buộc phân công đúng 2 reviewer
- Logic tổng hợp review
- Luồng publish đề tài
- Seed data rõ ràng để phục vụ demo
- Tài liệu hướng dẫn demo cho nhóm

## 3. Những thay đổi lớn đã được bổ sung

## 3.1 Mở rộng mô hình dữ liệu

Đã thêm entity mới:
- `RegistrationPeriod`

Đã mở rộng các entity hiện có:

### `Lecturer`
Đã thêm:
- `Title`
- `Specialization`

Ý nghĩa:
- Lưu học hàm/học vị hoặc danh xưng
- Lưu chuyên môn của giảng viên

### `Student`
Đã thêm:
- `StudentCode`
- `GroupRole`

Ý nghĩa:
- Lưu mã số sinh viên
- Lưu vai trò trong nhóm như `Leader`, `Member`

### `Topic`
Đã thêm:
- `TopicCode`
- `Description`
- `RegistrationPeriodId`
- `ReviewStatus`
- `PublicStatus`
- `CreatedAt`

Ý nghĩa:
- `TopicCode`: mã đề tài
- `Description`: mô tả hoặc nội dung chính của đề tài
- `RegistrationPeriodId`: đề tài thuộc đợt đăng ký nào
- `ReviewStatus`: trạng thái review (`Pending`, `Approved`, `Rejected`)
- `PublicStatus`: trạng thái public (`Private`, `Public`)
- `CreatedAt`: thời điểm tạo topic

### `TopicReview`
Đã thêm:
- `IsFinalized`

Ý nghĩa:
- Đánh dấu review đã hoàn tất hay chưa

## 3.2 Cập nhật DbContext và ràng buộc dữ liệu

Trong `CapstoneDbContext`, đã bổ sung:
- `DbSet<RegistrationPeriod>`
- Quan hệ giữa `Topic` và `RegistrationPeriod`
- Unique index cho `Topic.TopicCode`
- Unique index cho `(TopicId, ReviewerId)` trong `TopicReview`

Ý nghĩa:
- Không cho phép trùng mã đề tài
- Không cho phép một reviewer review cùng một topic nhiều lần

Ngoài ra đã cấu hình seed data phục vụ demo trực tiếp.

## 3.3 Thêm seed data phục vụ demo

Seed data hiện tại gồm:

### Semester
- `Spring 2026`

### RegistrationPeriod
- `Spring 2026 - Week 1` -> `Active`
- `Spring 2026 - Week 2` -> `Inactive`
- `Spring 2026 - Week 3` -> `Inactive`

Mỗi đợt kéo dài 1 tuần và chỉ có 1 đợt active tại một thời điểm.

### Lecturer
- `1 = Nguyen Van A`
- `2 = Tran Thi B`
- `3 = Le Van C`
- `4 = Pham Thi D`

### Admin
- `1 = System Admin`

### Student
- `1 = Pham Dinh Quoc Thinh`
- `2 = Nguyen Nhat Nam`

### Topic
- `1001` = đã approved và đã public
- `1002` = bị rejected và chưa public

### TopicReview
- Bộ review cho `1001` thể hiện luồng approved
- Bộ review cho `1002` thể hiện luồng rejected

Ý nghĩa:
- Có sẵn dữ liệu để test nhanh trên Swagger
- Không cần nhập toàn bộ dữ liệu bằng tay trước khi quay demo

## 4. Thay đổi ở tầng Services

## 4.1 TopicService được viết lại đầy đủ hơn

`TopicService` hiện không chỉ còn CRUD cơ bản mà đã xử lý được gần như toàn bộ luồng demo chính.

Các chức năng chính:
- Tạo topic
- Lấy tất cả topic
- Lấy chi tiết topic
- Cập nhật topic
- Xóa topic
- Gán reviewer cho topic
- Reviewer gửi kết quả review
- Tổng hợp kết quả review
- Publish topic
- Kiểm tra trùng đề tài ở mức cơ bản
- Trích xuất nội dung từ file docx/pdf ở mức MVP

## 4.2 Business validation khi tạo topic

Khi tạo topic, service kiểm tra:
- Giảng viên tạo đề tài có tồn tại hay không
- Đợt đăng ký có tồn tại và đang `Active` hay không
- `TopicCode` có bị trùng không
- Topic có bị đánh giá là rất giống với topic cũ không

Nếu vi phạm, hệ thống trả về lỗi và không tạo topic.

## 4.3 Logic phân công reviewer

Khi gọi API assign reviewer:
- 2 reviewer phải khác nhau
- Reviewer không được là chính người tạo topic
- Một topic không được assign lại reviewer nếu đã có review records

Nếu hợp lệ:
- Tạo 2 `TopicReview` ở trạng thái `Pending`
- Chuyển `Topic.Status` sang `InReview`

## 4.4 Logic review topic

Khi reviewer nộp review:
- Chỉ reviewer đã được assign mới được review
- `Decision` chỉ nhận `Approved` hoặc `Rejected`
- Review sẽ được cập nhật:
  - `Decision`
  - `Comment`
  - `ReviewDate`
  - `IsFinalized`

Sau đó hệ thống tự động gọi hàm tổng hợp trạng thái review của topic.

## 4.5 Logic tổng hợp kết quả review

Luật hiện tại:

### Trường hợp 1: Có ít nhất 1 reviewer reject
- `Topic.ReviewStatus = Rejected`
- `Topic.Status = Rejected`
- `Topic.PublicStatus = Private`

### Trường hợp 2: Cả 2 reviewer đều approve
- `Topic.ReviewStatus = Approved`
- `Topic.Status = Approved`

### Trường hợp 3: Chưa đủ kết quả
- `Topic.ReviewStatus = Pending`

## 4.6 Logic publish topic

Khi gọi API publish:
- Nếu `ReviewStatus != Approved` thì không cho publish
- Nếu hợp lệ:
  - `Topic.PublicStatus = Public`
  - `Topic.Status = Published`

Ngoài ra hệ thống trả về thông điệp giả lập gửi email cho creator.

## 5. Thay đổi ở tầng API

## 5.1 Chuẩn hóa route theo kiểu RESTful

Trước đó API còn dùng kiểu tên route mang tính thao tác như:
- `AddNewTopic`
- `GetAllTopic`
- `UpdateTopicData`

Hiện tại đã chuyển dần sang route RESTful rõ ràng hơn:
- `GET /api/topics`
- `GET /api/topics/{id}`
- `POST /api/topics`
- `PUT /api/topics/{id}`
- `DELETE /api/topics/{id}`

## 5.2 Bổ sung endpoint cho workflow chính

Các endpoint mới hoặc đã hoàn thiện:

### Registration periods
- `GET /api/registration-periods/active`
- `POST /api/registration-periods`

### Topics
- `GET /api/topics`
- `GET /api/topics/{id}`
- `POST /api/topics`
- `PUT /api/topics/{id}`
- `DELETE /api/topics/{id}`
- `POST /api/topics/{id}/assign-reviewers`
- `POST /api/topics/{id}/reviews`
- `GET /api/topics/{id}/review-summary`
- `POST /api/topics/{id}/publish`

### API phụ trợ
- `POST /api/topics/duplicate-check`
- `POST /api/topics/extract-from-file`

## 6. Thay đổi liên quan đến migration và database

Đã thêm và cập nhật migration để phản ánh schema mới:
- Thêm entity `RegistrationPeriod`
- Thêm field mới vào `Topic`, `Lecturer`, `Student`, `TopicReview`
- Thêm seed data phục vụ demo

Mục tiêu của phần này:
- Đảm bảo database khớp với mô hình entity hiện tại
- Cho phép demo trực tiếp sau khi chạy `database update`

## 7. Tài liệu và file phục vụ demo đã được bổ sung

Đã thêm các file:

### `CapstoneProjectRegistration.API/CapstoneProjectRegistration.Demo.http`
Mục đích:
- Chứa sẵn request mẫu để test nhanh
- Có cả happy path và reject path

### `docs/demo-video-script.md`
Mục đích:
- Hướng dẫn thành viên nhóm quay video demo theo thứ tự cụ thể

### `docs/api-handover-guide.md`
Mục đích:
- Tài liệu bàn giao để nhóm nắm toàn bộ API, seed data và business rule

### `docs/project-change-summary.md`
Mục đích:
- Tóm tắt đầy đủ các thay đổi code, logic và trạng thái dự án hiện tại

## 8. Luồng nghiệp vụ hiện tại đang chạy như thế nào

Luồng demo chuẩn hiện tại:

1. Gọi `GET /api/registration-periods/active`
   - Lấy đợt đăng ký đang mở

2. Gọi `POST /api/topics`
   - Giảng viên tạo đề tài mới
   - Topic mới có trạng thái `Pending`

3. Gọi `POST /api/topics/{id}/assign-reviewers`
   - Admin phân công 2 reviewer
   - Topic chuyển sang `InReview`

4. Gọi `POST /api/topics/{id}/reviews`
   - Reviewer thứ nhất gửi kết quả

5. Gọi `POST /api/topics/{id}/reviews`
   - Reviewer thứ hai gửi kết quả

6. Gọi `GET /api/topics/{id}/review-summary`
   - Xem kết quả tổng hợp

7. Gọi `POST /api/topics/{id}/publish`
   - Nếu approved thì publish thành công
   - Nếu rejected thì publish thất bại

## 9. Trạng thái hiện tại của dự án

Hiện tại backend đã đủ để demo tốt phần RESTful API cốt lõi:
- Có migration
- Có update database
- Có seed data
- Có Swagger
- Có request mẫu
- Có luồng happy path
- Có luồng reject path để chứng minh business rule

Các phần chưa phải trọng tâm hiện tại:
- Authentication / Authorization hoàn chỉnh
- Gửi email thật
- Duplicate-check nâng cao bằng AI
- Import DOC/PDF hoàn chỉnh cho production

## 10. Kết luận

Tính đến hiện tại, dự án đã chuyển từ mức khung cơ bản sang mức backend có thể demo nghiệp vụ thực tế.

Những điểm nổi bật nhất đã hoàn thành:
- Chuẩn hóa dữ liệu cho bài toán capstone
- Hoàn thiện luồng review và publish đề tài
- Bổ sung seed data để demo nhanh
- Bổ sung tài liệu cho nhóm và video demo

Nói ngắn gọn, hệ thống hiện tại đã đủ cơ sở để:
- Demo trên Swagger
- Chụp ảnh endpoint
- Quay video luồng nghiệp vụ
- Bàn giao cho các thành viên trong nhóm tiếp tục phát triển
