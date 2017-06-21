var express = require('express');
var path = require('path');
var app = express();

// app.get("/", function(req, res) {
//     res.send("Basic Server");
// });

// register mithril-express as the view engine taht should be used for all .js files
app.engine('js', require('mithril-express'));

// set default view engine to .js
app.set('view engine', 'js');
// define views directory
app.set('views', path.join(__dirname + '/src/views'));

app.get('/', function(req, res) {
    res.render('index');
});
// Server
app.listen(3000, function() {
    console.log("Server started on port 3000.");
});
