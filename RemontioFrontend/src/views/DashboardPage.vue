<template>Dashboard Page - work in progress</template>
<script setup lang="ts">
import { onMounted } from 'vue'
import { Backend } from '@/main'
onMounted(async () => {
  console.log(await Backend.getProjectList())
})
</script>
