import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', component: () => import('./views/LandingView.vue') },
    { path: '/resume', component: () => import('./views/ResumeView.vue') },
    { path: '/analyze', component: () => import('./views/AnalyzeView.vue') },
    { path: '/result/:id', component: () => import('./views/ResultView.vue'), props: true },
    { path: '/history', component: () => import('./views/HistoryView.vue') }
  ]
})

export default router
