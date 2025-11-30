let currentQuizId = null;

// Amikor az oldal betölt, lekérjük a kvízek listáját
document.addEventListener("DOMContentLoaded", loadQuizzes);

async function loadQuizzes() {
    try {
        // Mivel egy szerveren vagyunk, elég a relatív útvonal!
        const response = await fetch('/Quiz/list');
        const quizzes = await response.json();

        const listDiv = document.getElementById('quiz-list');
        listDiv.innerHTML = '';

        if (quizzes.length === 0) {
            listDiv.innerHTML = '<p>Nincs elérhető kvíz.</p>';
            return;
        }

        // Gombok generálása minden kvízhez
        quizzes.forEach(quiz => {
            const btn = document.createElement('button');
            btn.textContent = quiz.title || quiz.name; // A DTO-ban Title vagy Name van? (igazodjunk a C#-hoz)
            btn.style.marginRight = "10px";
            btn.onclick = () => renderQuiz(quiz);
            listDiv.appendChild(btn);
        });

    } catch (error) {
        console.error("Hiba a betöltéskor:", error);
        document.getElementById('quiz-list').innerText = "Hiba a szerver elérésekor.";
    }
}

function renderQuiz(quiz) {
    currentQuizId = quiz.id;
    document.getElementById('active-quiz-container').style.display = 'block';
    document.getElementById('quiz-title').textContent = quiz.title || quiz.name;
    document.getElementById('result').textContent = '';

    const container = document.getElementById('questions-container');
    container.innerHTML = '';

    // Kérdések kirajzolása
    quiz.questions.forEach((q, qIndex) => {
        const qDiv = document.createElement('div');
        qDiv.className = 'question';
        qDiv.textContent = `${qIndex + 1}. ${q.text}`;
        container.appendChild(qDiv);

        const optionsDiv = document.createElement('div');
        optionsDiv.className = 'options';

        // Válaszlehetőségek (Radio gombok)
        q.options.forEach((opt, optIndex) => {
            const label = document.createElement('label');
            label.style.display = 'block';

            const radio = document.createElement('input');
            radio.type = 'radio';
            radio.name = `question_${qIndex}`; // Fontos: kérdésenként egyedi név
            radio.value = optIndex; // Az indexet küldjük vissza (0, 1, 2...)

            label.appendChild(radio);
            label.appendChild(document.createTextNode(` ${opt}`));
            optionsDiv.appendChild(label);
        });

        container.appendChild(optionsDiv);
    });
}

async function submitQuiz() {
    if (currentQuizId === null) return;

    // Összeszedjük a válaszokat
    const answers = [];
    const questionsContainer = document.getElementById('questions-container');
    // Megszámoljuk hány kérdés volt (a .options div-ek száma alapján)
    const questionCount = questionsContainer.querySelectorAll('.options').length;

    for (let i = 0; i < questionCount; i++) {
        const selected = document.querySelector(`input[name="question_${i}"]:checked`);
        if (selected) {
            answers.push(parseInt(selected.value));
        } else {
            answers.push(-1); // Ha nem válaszolt, -1-et küldünk (vagy kezelheted hibaként)
        }
    }

    // Küldés a szervernek
    const payload = {
        quizId: currentQuizId,
        selectedAnswerIndices: answers
    };

    try {
        const response = await fetch('/Quiz/submit', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            alert("Hiba történt a kiértékeléskor!");
            return;
        }

        const result = await response.json();

        // Eredmény megjelenítése
        document.getElementById('result').textContent =
            `Pontszám: ${result.score} / ${result.totalQuestions} - ${(result.score / result.totalQuestions) * 100}%`;

    } catch (error) {
        console.error(error);
    }
}