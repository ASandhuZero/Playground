// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import router from './router/index.js'
// const express = require('express')
// const path = require('path')
// const expressVue = require('express-vue')
// Vue.config.productionTip = false
//
// const app = express()
// const vueOptions = {
//   rootPath: path.join(__dirname, './components'),
//   layout: {
//     start: '<div id="app">',
//     end: '</div>'
//   }
// }
//
// const expressVueMiddleware = expressVue.init(vueOptions)
// app.use(expressVueMiddleware)
/* eslint-disable no-new */
new Vue({
  router,
  el: '#app',
  template: '<App/>',
  components: { App }
})
