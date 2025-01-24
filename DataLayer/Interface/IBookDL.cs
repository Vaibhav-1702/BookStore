using Model.DTO;
using Model.Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interface
{
    public interface IBookDL
    {
        public Task<ResponseModel<Book>> AddBook(BookDto bookDto);

        public Task<ResponseModel<IEnumerable<Book>>> GetAllBooks();

        public  Task<ResponseModel<Book>> GetBookById(int bookId);

        public Task<ResponseModel<bool>> DeleteBookById(int bookId);

        public Task<ResponseModel<Book>> UpdateBook(int bookId, UpdateBookDto updateBookDto);

    }
}
