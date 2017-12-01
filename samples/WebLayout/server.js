//"use strict";

//var http = require('http');
//var port = process.env.PORT || 1337;

//http.createServer(function (req, res) {
//    res.writeHead(200, { 'Content-Type': 'text/plain' });
//    res.end('Hello World\n');
//}).listen(port);

//var server = require('server-js');
//server.static(".");
//var app = server.start();

//var express = require('express');
//app = express();
//app.use("/", express.static("./"));
//app.listen(1337);

var http = require("http");
var fs = require("fs");

const port = 1337;

fs.readFile('./index.html', function (err, html) {

    if (err) {
         throw err;
    }

    http.createServer(function (request, response) {
        response.writeHeader(200, { "Content-Type": "text/html" });
        response.write(html);
        response.end();
    }).listen(port);
});