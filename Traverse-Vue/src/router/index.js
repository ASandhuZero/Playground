import Vue from 'vue'
import VueRouter from 'vue-router'
import Hello from '@/components/Hello'
import Home from '@/components/Home'
import About from '@/components/About'
import Play from '@/components/Play'

Vue.use(VueRouter)

const router = new VueRouter({
  mode: 'history',
  base: __dirname,
  routes: [
    {path: '/', component: Home},
    {path: '/About', component: About},
    {path: '/Hello', component: Hello},
    {path: '/Play', component: Play}
  ]
})

export default router
