import Vue from 'vue'
import Router from 'vue-router'
import Test from '@/components/Test.vue'
import Hello from '@/components/Hello'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'Test',
      component: Test
    },
    {
      path: '/Hello',
      name: 'Hello',
      component: Hello
    }
  ]
})
