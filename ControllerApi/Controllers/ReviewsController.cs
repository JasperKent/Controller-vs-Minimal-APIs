using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControllerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookReviewsController : ControllerBase
    {
        private readonly ReviewContext _reviewContext;

        private static bool ValidateReview(BookReview bookReview)
        {
            return !string.IsNullOrWhiteSpace(bookReview.Title)
                && bookReview.Rating >= 1
                && bookReview.Rating <= 5;
        }

        public BookReviewsController(ReviewContext reviewContext)
        {
            _reviewContext = reviewContext;
        }

        /// <summary>
        /// Gets all book reviews
        /// </summary>
        /// <returns>The collection of reviews</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BookReview>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task <IActionResult> Get()
        {
            return Ok(await _reviewContext.BookReviews.ToArrayAsync());
        }

        /// <summary>
        /// Gets a single book review by id
        /// </summary>
        /// <param name="id">The id of the book review</param>
        /// <returns>The book review</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookReview))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _reviewContext.BookReviews.FindAsync(id);

            if (result == null)
                return NotFound();
            else
                return Ok(result);
        }

        /// <summary>
        /// Gets a summary of the reviews for each book with an average rating
        /// </summary>
        /// <returns>The summary of book reviews</returns>
        [HttpGet("summary")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BookReview>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Summary()
        {
            var summaries = await _reviewContext.BookReviews.GroupBy(r => r.Title).Select(g =>
                new BookReview
                {
                    Title = g.Key,
                    Rating = Math.Round(g.Average(r => r.Rating), 2)
                }).ToListAsync();

            int id = 1;

            summaries.ForEach(r => r.Id = id++);

            return Ok(summaries);
        }

        /// <summary>
        /// Creates a new book review
        /// </summary>
        /// <param name="review">The new book review</param>
        /// <returns>A Created response</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] BookReview review)
        {
            if (!ValidateReview(review))
                return UnprocessableEntity();

            _reviewContext.BookReviews.Add(review);
            
            await _reviewContext.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = review.Id }, review);
        }

        /// <summary>
        /// Modifies and existing book review
        /// </summary>
        /// <param name="id">The id of the review to modify</param>
        /// <param name="review">The updated review</param>
        /// <returns>Ok or NotFound</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id, [FromBody] BookReview review)
        {
            if (!ValidateReview(review))
                return UnprocessableEntity();

            var result = await _reviewContext.BookReviews.FindAsync(id); 

            if (result == null)
                return NotFound();
            else
            {
                result.Rating = review.Rating;
                result.Title = review.Title;

                await _reviewContext.SaveChangesAsync();

                return Ok();
            }
        }

        /// <summary>
        /// Deletes a book review
        /// </summary>
        /// <param name="id">The id of the review to delete</param>
        /// <returns>Ok or NotFound</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _reviewContext.BookReviews.FindAsync(id);

            if (result == null)
                return NotFound();
            else
            {
                _reviewContext.BookReviews.Remove(result);

                await _reviewContext.SaveChangesAsync();

                return Ok();
            }
        }
    }
}