using CuaHangBanDoOnline.Models;
using CuaHangBanDoOnline.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CuaHangBanDoOnline.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManagerController : Controller
    {
        private readonly IManagerRepository _managerRepository;

        public ManagerController(IManagerRepository managerRepository)
        {
            _managerRepository = managerRepository;
        }

        // GET: /Manager/Index
        [HttpGet]
        public IActionResult Index(string searchEmail)
        {
            try
            {
                var users = _managerRepository.SearchUsersByEmail(searchEmail);
                ViewBag.SearchEmail = searchEmail; // Lưu giá trị tìm kiếm để hiển thị lại trên form
                return View(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có lỗi xảy ra khi tải danh sách người dùng: " + ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _managerRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.MaVaiTro == (int)RoleType.Admin)
            {
                TempData["Error"] = "Không được phép chỉnh sửa tài khoản Admin.";
                return RedirectToAction("Index");
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User user)
        {
            var existingUser = _managerRepository.GetUserById(user.MaNguoiDung);
            if (existingUser == null)
            {
                return NotFound();
            }

            if (existingUser.MaVaiTro == (int)RoleType.Admin)
            {
                TempData["Error"] = "Không được phép chỉnh sửa tài khoản Admin.";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                existingUser.TenDangNhap = user.TenDangNhap;
                existingUser.Email = user.Email;
                existingUser.MatKhau = user.MatKhau; // Nên mã hóa mật khẩu trong thực tế
                _managerRepository.UpdateUser(existingUser);

                TempData["Message"] = "Cập nhật tài khoản thành công.";
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: /Manager/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var user = _managerRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.MaVaiTro == (int)RoleType.Admin)
            {
                TempData["Error"] = "Không được phép xóa tài khoản Admin.";
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // POST: /Manager/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = _managerRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.MaVaiTro == (int)RoleType.Admin)
            {
                TempData["Error"] = "Không được phép xóa tài khoản Admin.";
                return RedirectToAction("Index");
            }

            _managerRepository.DeleteUser(id);
            TempData["Message"] = "Xóa tài khoản thành công.";
            return RedirectToAction("Index");
        }

        // POST: /Manager/ChangeRole
        [HttpPost]
        public IActionResult ChangeRole(int maNguoiDung, string newRole)
        {
            var user = _managerRepository.GetUserById(maNguoiDung);
            if (user == null)
            {
                return NotFound();
            }

            if (user.MaVaiTro == (int)RoleType.Admin)
            {
                TempData["Error"] = "Không được phép thay đổi vai trò của tài khoản Admin.";
                return RedirectToAction("Index");
            }

            int newRoleId = newRole switch
            {
                "Staff" => (int)RoleType.Staff,
                "Customer" => (int)RoleType.Customer,
                _ => (int)RoleType.Customer
            };

            _managerRepository.ChangeUserRole(maNguoiDung, newRoleId);
            TempData["Message"] = "Thay đổi vai trò thành công.";
            return RedirectToAction("Index");
        }
    }
}