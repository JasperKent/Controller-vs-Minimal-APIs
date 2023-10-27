using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MinimalApi.Endpoints
{
    internal static class BookReviewEndpoints
    {
        private static bool ValidateReview(BookReview bookReview)
        {
            return !string.IsNullOrWhiteSpace(bookReview.Title)
                && bookReview.Rating >= 1
                && bookReview.Rating <= 5;
        }

        /// <summary>
        /// Gets all book reviews
        /// </summary>
        /// <returns>The collection of reviews</returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BookReview>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public static async Task<IResult> GetAll(ReviewContext reviewContext)
        {
            return TypedResults.Ok(await reviewContext.BookReviews.ToArrayAsync());
        }

        /// <summary>
        /// Gets a summary of the reviews for each book with an average rating
        /// </summary>
        /// <returns>The summary of book reviews</returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BookReview>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public static async Task<IResult> Summary(ReviewContext reviewContext)
        {
            var summaries = await reviewContext.BookReviews.GroupBy(r => r.Title).Select(g =>
            new BookReview
            {
                Title = g.Key,
                Rating = Math.Round(g.Average(r => r.Rating), 2)
            }).ToListAsync();

            int id = 1;

            summaries.ForEach(r => r.Id = id++);

            return TypedResults.Ok(summaries);
        }

        /// <summary>
        /// Gets a single book review by id
        /// </summary>
        /// <param name="id">The id of the book review</param>
        /// <returns>The book review</returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookReview))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public static async Task<IResult> GetOne(ReviewContext reviewContext, int id)
        {
            var result = await reviewContext.BookReviews.FindAsync(id);

            if (result == null)
                return TypedResults.NotFound();
            else
                return TypedResults.Ok(result);
        }

        /// <summary>
        /// Creates a new book review
        /// </summary>
        /// <param name="review">The new book review</param>
        /// <returns>A Created response</returns>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public static async Task<IResult> Post(ReviewContext reviewContext, [FromBody] BookReview review)
        {
            if (!ValidateReview(review))
                return TypedResults.UnprocessableEntity();

            reviewContext.BookReviews.Add(review);

            await reviewContext.SaveChangesAsync();

            return TypedResults.CreatedAtRoute(
                routeName: "GetOne", 
                routeValues: new { id = review.Id }, 
                value: review);
        }

        /// <summary>
        /// Modifies and existing book review
        /// </summary>
        /// <param name="id">The id of the review to modify</param>
        /// <param name="review">The updated review</param>
        /// <returns>Ok or NotFound</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public static async Task<IResult> Put(ReviewContext reviewContext, int id, [FromBody] BookReview review)
        {
            if (!ValidateReview(review))
                return TypedResults.UnprocessableEntity();

            var result = await reviewContext.BookReviews.FindAsync(id); 

            if (result == null)
                return TypedResults.NotFound();
            else
            {
                result.Rating = review.Rating;
                result.Title = review.Title;

                await reviewContext.SaveChangesAsync();

                return TypedResults.Ok();
            }
        }

        /// <summary>
        /// Deletes a book review
        /// </summary>
        /// <param name="id">The id of the review to delete</param>
        /// <returns>Ok or NotFound</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public static async Task<IResult> Delete(ReviewContext reviewContext, int id)
        {
            var result = await reviewContext.BookReviews.FindAsync(id);

            if (result == null)
                return TypedResults.NotFound();
            else
            {
                reviewContext.BookReviews.Remove(result);

                await reviewContext.SaveChangesAsync();

                return TypedResults.Ok();
            }
        }
    }
}
