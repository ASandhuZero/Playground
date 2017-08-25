import Vue from 'vue'
import VueRouter from 'vue-router'
import Hello from '@/components/Hello'
import Home from '@/components/Home'
import About from '@/components/About'

Vue.use(VueRouter)

const router = new VueRouter({
  mode: 'history',
  base: __dirname,
  routes: [
    {path: '/', component: Home},
    {path: '/About', component: About},
    {path: '/Hello', component: Hello}
  ]
})

export default router
// export default new Router({
//   routes: [
//     {
//       path: '/Hello',
//       name: 'Hello',
//       component: Hello
//     },
//     {
//       path: '/',
//       name: 'Home',
//       component: Home
//     },
//     {
//       path: '/About',
//       name: 'About',
//       component: About
//     }
//   ]
// })
