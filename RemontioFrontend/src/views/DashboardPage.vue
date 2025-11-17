<template>
  <MainLayout>THis is a dashobard </MainLayout>
</template>
<script setup lang="ts">
import { onMounted } from 'vue'
import { Backend } from '@/main'
import MainLayout from './layouts/MainLayout.vue'
onMounted(async () => {
  console.log(await Backend.getProjectList())
})
</script>
