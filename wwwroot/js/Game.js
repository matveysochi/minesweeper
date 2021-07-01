'use strict';

window.addEventListener('selectstart', event => event.preventDefault());
window.addEventListener('contextmenu', event => event.preventDefault())

let modeBtns = document.querySelectorAll('#mode button');
let levelBtns = document.querySelectorAll('#level button');

let gameArea = document.querySelector('#game');
let statArea = document.querySelector('#statistics');

let privateBtn = document.querySelector('#privateBtn');
let publicBtn = document.querySelector('#publicBtn');

let statTable = document.querySelector('#statTable');

let attempts = document.querySelector('#attempts');
let wins = document.querySelector('#wins');
let losses = document.querySelector('#losses');
let average = document.querySelector('#average');
let best = document.querySelector('#best');
let place = document.querySelector('#position');

let privateStat = document.querySelector('#privateStat');
let publicStat = document.querySelector('#publicStat');

let privateRecords = document.querySelector('#privateRecords');
let publicRecords = document.querySelector('#publicRecords')

let timerElement = document.querySelector('#time');

let minesElement = document.querySelector('#mines');

let field = document.querySelector('#field');

let tableCells = [];

let time;
let timerId;
let timeLimit = 99 * 60 + 59;

let currentLevel = 0;

configBtns(levelBtns, level => {
    currentLevel = level;
    NewGame(level)
    RefreshStat(level);
});

configBtns(modeBtns, mode => {
    gameArea.hidden = mode != 0;
    statArea.hidden = mode == 0;
    RefreshStat(currentLevel);
})

privateBtn.addEventListener('click', () => ChangeStat(true))
publicBtn.addEventListener('click', () => ChangeStat(false))
timerElement.addEventListener('click', () => NewGame(currentLevel))

GetState();
RefreshStat(currentLevel);


function configBtns(btns, callback) {
    btns.forEach(btn => btn.addEventListener('click', () => {
        let value = btn.dataset.value;
        btns.forEach((b, index) => ChangeButtonView(b, value == index))
        callback(value);
    }));
}

function ChangeButtonView(btn, primary) {
    if (primary) {
        btn.classList.add('btn-primary');
        btn.classList.remove('btn-secondary');
    } else {
        btn.classList.remove('btn-primary');
        btn.classList.add('btn-secondary');
    }
}

function ChangeStat(flag) {
    ChangeButtonView(privateBtn, flag);
    ChangeButtonView(publicBtn, !flag);

    privateStat.hidden = !flag;
    publicStat.hidden = flag;
}

async function RefreshStat(level) {

    let response = await fetch(`game/statistics/?type=${level}`);
    let stat = await response.json();

    if (stat.attempts == 0) {
        statTable.hidden = true;
    } else {
        statTable.hidden = false;

        attempts.innerHTML = stat.attempts;
        wins.innerHTML = `${stat.wins} (${Math.floor((stat.wins / stat.attempts) * 100)}%)`;
        losses.innerHTML = `${stat.losses} (${Math.floor((stat.losses / stat.attempts) * 100)}%)`;
        average.innerHTML = TimeFromSpan(stat.averageTime);
        best.innerHTML = TimeFromSpan(stat.bestTime);
        place.innerHTML = stat.place;
    }

    AddRecords(privateRecords, stat.privateRecords);
    AddRecords(publicRecords, stat.publicRecords);
}

function AddRecords(table, records) {
    table.innerHTML = '';
    records.forEach((record, index) => {
        let tr = document.createElement('tr');
        table.append(tr);

        let td = document.createElement('td');
        tr.append(td);
        td.innerHTML = index + 1;

        for (let key in record) {
            let td = document.createElement('td');
            tr.append(td);
            td.innerHTML = record[key];
        }
    });
}


async function NewGame(level) {
    clearInterval(timerId);

    let searchParams = new URLSearchParams();
    searchParams.set('type', level);

    let response = await fetch(`game/new/`, {
        method: 'POST',
        body: searchParams
    });
    let model = await response.json();

    refreshField(model);
}

async function Action(event) {
    if (time >= timeLimit) {
        clearInterval(timerId);
        return;
    }

    let x = this.dataset.x;
    let y = this.dataset.y;
    let action = event.ctrlKey ? 1 : event.button;

    let searchParams = new URLSearchParams();
    searchParams.set('x', x);
    searchParams.set('y', y);
    searchParams.set('action', action);

    let response = await fetch(`game/action/`, {
        method: 'POST',
        body: searchParams
    });

    let model = await response.json();

    if (time == 0) {
        time = 0.001;
        timerId = setInterval(tick, 1000);
    }
    if (model.gameEnd) {
        clearInterval(timerId);
    }
    refreshTimeView(model.gameEnd, model.win);

    let openCount = 0;
    let flagCount = 0;
    let cells = model.cells;

    for (let y in tableCells) {
        for (let x in tableCells[y]) {
            let td = tableCells[y][x];
            let cell = cells[y][x];
            td.setAttribute('class', getClassString(cell));
            if (cell.bombAround > 0 && cell.isOpen && !cell.isBomb) {
                td.innerHTML = cell.bombAround;
            } else {
                td.innerHTML = '';
            }
            if (cell.isOpen) openCount++;
            if (cell.isFlag) flagCount++;
        }
    }
    RefreshMinesView(model.type, openCount, flagCount)
}

async function GetState() {
    let response = await fetch('game/state/');
    let model = await response.json();

    refreshField(model)
}

function refreshField(model) {
    field.innerHTML = "";

    if (model.gameStart && !model.gameEnd) timerId = setInterval(tick, 1000);
    time = model.time;
    refreshTimeView(model.gameEnd, model.win);

    let openCount = 0;
    let flagCount = 0;

    tableCells = [];

    for (let y in model.cells) {

        let tr = document.createElement('tr');
        field.append(tr);

        let row = [];
        tableCells.push(row);

        for (let x in model.cells[y]) {
            let cell = model.cells[y][x];

            let td = document.createElement('td');
            tr.append(td);
            td.addEventListener('mouseup', Action);
            td.setAttribute('class', getClassString(cell));
            td.dataset.x = x;
            td.dataset.y = y;
            if (cell.bombAround > 0 && cell.isOpen && !cell.isBomb) {
                td.innerHTML = cell.bombAround;
            }
            row.push(td);
            if (cell.isOpen) openCount++;
            if (cell.isFlag) flagCount++;
        }
    }
    RefreshMinesView(model.type, openCount, flagCount)
}


function tick() {
    time = time >= timeLimit ? timeLimit : time + 1
    refreshTimeView();
}

function RefreshMinesView(type, openCount, flagCount) {
    let restToOpen = NormNum(type.width * type.height - openCount - type.bombCount, 3);
    let restFlag = NormNum(type.bombCount - flagCount);
    minesElement.innerHTML = `${restFlag}/${restToOpen}`;
}

function refreshTimeView(end, win) {
    let min = NormNum(Math.floor(time / 60));
    let sec = NormNum(Math.floor(time % 60));
    timerElement.innerHTML = `${min}:${sec}`;
    if (end) {
        timerElement.classList.remove('btn-outline-secondary');
        if (win) {
            timerElement.classList.add('btn-success')
        } else {
            timerElement.classList.add('btn-danger')
        }
    } else {
        timerElement.classList.add('btn-outline-secondary');
        timerElement.classList.remove('btn-success')
        timerElement.classList.remove('btn-danger')
    }
}

function TimeFromSpan(timeSpan) {
    return `${NormNum(timeSpan.totalMinutes)}:${NormNum(timeSpan.seconds)}`
}
function NormNum(dig, count=2) {
    dig = Math.floor(dig);
    for (let i = 0; i < count - String(dig).length; i++) dig = '0' + dig;
    return dig;
}

function getClassString(cell) {
    return "cell " +
        (cell.isBomb ? "bomb " : "") +
        (cell.isOpen ? "open " : "close ") +
        (cell.isFlag ? "flag " : "") +
        `color${cell.bombAround}`;
}