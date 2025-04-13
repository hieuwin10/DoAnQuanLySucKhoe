# Hướng dẫn triển khai cơ sở dữ liệu

## Tổng quan

Tài liệu này hướng dẫn cách triển khai cơ sở dữ liệu cho hệ thống quản lý sức khỏe cá nhân. Cơ sở dữ liệu được thiết kế để lưu trữ thông tin về người dùng, hồ sơ sức khỏe, chỉ số sức khỏe, lịch hẹn, tư vấn, và nhiều thông tin khác liên quan đến quản lý sức khỏe.

## Cấu trúc cơ sở dữ liệu

Cơ sở dữ liệu bao gồm 15 bảng chính:

1. `vai_tro`: Lưu trữ các vai trò trong hệ thống (Admin, Bác sĩ, Bệnh nhân)
2. `nguoi_dung`: Lưu trữ thông tin người dùng (đã được thay thế bằng ApplicationUser)
3. `ho_so_suc_khoe`: Lưu trữ thông tin sức khỏe cơ bản của người dùng
4. `chi_so_suc_khoe`: Lưu trữ lịch sử các chỉ số sức khỏe theo thời gian
5. `nhac_nho_suc_khoe`: Lưu trữ các nhắc nhở sức khỏe cho người dùng
6. `ke_hoach_dinh_duong`: Lưu trữ kế hoạch dinh dưỡng do chuyên gia tạo
7. `chi_tiet_ke_hoach_dinh_duong`: Lưu trữ chi tiết các món ăn trong kế hoạch dinh dưỡng
8. `ke_hoach_tap_luyen`: Lưu trữ kế hoạch tập luyện do chuyên gia tạo
9. `chi_tiet_ke_hoach_tap_luyen`: Lưu trữ chi tiết các bài tập trong kế hoạch tập luyện
10. `tu_van_suc_khoe`: Lưu trữ các câu hỏi tư vấn sức khỏe và câu trả lời
11. `lich_hen`: Lưu trữ lịch hẹn giữa bệnh nhân và chuyên gia
12. `tin_nhan`: Lưu trữ tin nhắn giữa các người dùng
13. `danh_gia_chuyen_gia`: Lưu trữ đánh giá của bệnh nhân về chuyên gia
14. `thong_bao_he_thong`: Lưu trữ thông báo hệ thống do admin tạo
15. `phan_hoi_nguoi_dung`: Lưu trữ phản hồi của người dùng về hệ thống

## Mối quan hệ chính

- **Người dùng - Vai trò**: Mỗi người dùng có một vai trò (Admin, Bác sĩ, Bệnh nhân)
- **Bệnh nhân - Hồ sơ sức khỏe**: Mỗi bệnh nhân có một hồ sơ sức khỏe
- **Bệnh nhân - Chỉ số sức khỏe**: Mỗi bệnh nhân có nhiều chỉ số sức khỏe theo thời gian
- **Bệnh nhân - Bác sĩ (Tư vấn)**: Bệnh nhân đặt câu hỏi, bác sĩ trả lời
- **Bệnh nhân - Bác sĩ (Lịch hẹn)**: Bệnh nhân đặt lịch hẹn với bác sĩ
- **Bệnh nhân - Bác sĩ (Đánh giá)**: Bệnh nhân đánh giá bác sĩ
- **Bác sĩ - Kế hoạch dinh dưỡng/tập luyện**: Bác sĩ tạo kế hoạch cho bệnh nhân

## Các bước triển khai

Xem chi tiết trong các tài liệu hướng dẫn:

- [Hướng dẫn triển khai frontend](frontend.md)
- [Hướng dẫn triển khai backend](backend.md)

## Mã nguồn

Các file mã nguồn cần thiết được cung cấp trong thư mục `code/`:

- Models cho cơ sở dữ liệu
- ApplicationDbContext
- Migrations
- Các file cấu hình Entity Framework Core
