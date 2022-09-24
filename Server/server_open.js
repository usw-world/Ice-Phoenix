const express = require("express");
const mysql = require('mysql');
const fs = require('fs');
const connection = mysql.createConnection({
    host: "ec2-13-125-132-123.ap-northeast-2.compute.amazonaws.com",
    user: "root",
    password: "1234",
    database: "Project"
});

const app = express();

app.use(express.static('public'));
app.use(express.urlencoded({extended: true}));

app.post('/selectUser', (req, res) => {
    var CurrentUserNum = req.body.CurrentUserNum;
     connection.query('SELECT * from Project WHERE id = ?', CurrentUserNum,  (err, rows) => {
      if (err)
        console.log(err);
      else
      {
        fs.writeFileSync("userdata.json", JSON.stringify({ 'users' : rows}));
        res.send(rows[0]);
      }
       
     });
 });

 app.post('/insertUser', (req, res) => { // 로그인 기능이기 때문에 중복 확인 기능이 필요.
    var registerName = req.body.registerName;
    connection.query('INSERT INTO Project VALUES(null, ?)', registerName, (err, rows) => {
        if (err) 
            console.log(err);
    });
 });

app.listen(3301, function(){
    console.log("Connected 3301 server");
});
