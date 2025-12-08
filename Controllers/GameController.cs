using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class GameController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GameController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= INDEX =================
        public IActionResult Index()
        {
            var gameLevels = _context.GameLevels.ToList();
            return View(gameLevels);
        }

        // ================= CREATE =================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(GameLevel gameLevel)
        {
            if (ModelState.IsValid)
            {
                _context.GameLevels.Add(gameLevel);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gameLevel);
        }

        // ================= EDIT =================
        public IActionResult Edit(int id)
        {
            var gameLevel = _context.GameLevels.Find(id);
            return View(gameLevel);
        }

        [HttpPost]
        public IActionResult Edit(GameLevel gameLevel)
        {
            if (ModelState.IsValid)
            {
                _context.GameLevels.Update(gameLevel);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gameLevel);
        }

        // ================= DELETE =================
        public IActionResult Delete(int id)
        {
            var gameLevel = _context.GameLevels.Find(id);
            _context.GameLevels.Remove(gameLevel!);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        // ================= DETAILS QUESTION =================
public IActionResult Details(int id)
{
    var gameLevel = _context.GameLevels.Find(id);

    ViewBag.GameLevelTitle = gameLevel!.title;
    ViewBag.GameLevelId = gameLevel.levelId;

    var questions = _context.Questions
        .Where(q => q.levelId == id)
        .ToList();

    return View(questions);
}

// ================= CREATE QUESTION =================
public IActionResult CreateQuestion(int levelId)
{
    Question question = new Question
    {
        levelId = levelId
    };
    return View(question);
}

[HttpPost]
public IActionResult CreateQuestion(Question question)
{
    if (ModelState.IsValid)
    {
        _context.Questions.Add(question);
        _context.SaveChanges();
        return RedirectToAction("Details", new { id = question.levelId });
    }

    return View(question);
}

// ================= EDIT QUESTION =================
public IActionResult EditQuestion(int QuestionId)
{
    var question = _context.Questions.Find(QuestionId);
    return View(question);
}

[HttpPost]
public IActionResult EditQuestion(Question question)
{
    if (ModelState.IsValid)
    {
        _context.Questions.Update(question);
        _context.SaveChanges();
        return RedirectToAction("Details", new { id = question.levelId });
    }

    return View(question);
}

// ================= DELETE QUESTION =================
public IActionResult DeleteQuestion(int QuestionId)
{
    var question = _context.Questions.Find(QuestionId);
    int levelId = question!.levelId;

    _context.Questions.Remove(question);
    _context.SaveChanges();

    return RedirectToAction("Details", new { id = levelId });
}

    }
}
