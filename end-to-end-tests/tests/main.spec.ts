import { test, expect } from '@playwright/test'

test('main ui test', async ({ page }) => {
  // Navigate to the login page
  await page.goto('https://localhost:7070/')

  // Fill in the login form and submit
  await page.getByLabel('Email *').click()
  await page.getByLabel('Email *').fill('user@mail.com')
  await page.getByLabel('Email *').press('Tab')
  await page.getByLabel('Password *').fill('123456')
  await page.getByRole('button', { name: 'Login' }).click()

  // Verify that the user is redirected to the home page after login
  await expect(page.getByRole('heading')).toContainText('Home page')

  // Navigate to the Dashboard page
  await page.getByRole('link', { name: 'Dashboard' }).click()
  await expect(page.getByRole('heading')).toContainText('Dashboard')

  // Navigate to the Page
  await page.getByRole('link', { name: 'Page' }).click()
  await expect(page.getByRole('heading')).toContainText('Page')

  // Click the first button twice
  await page.getByRole('button').first().click()
  await page.getByRole('button').first().click()

  // Navigate to the Change password page
  await page.getByLabel('User account').click()
  await page.getByRole('menuitem', { name: 'Change password' }).click()

  // Fill in the change password form and submit
  await page.getByLabel('Current password *').click()
  await page.getByLabel('Current password *').fill('123456')
  await page.getByLabel('New password *').click()
  await page.getByLabel('New password *').fill('654321')
  await page.getByLabel('New password confirmation *').click()
  await page.getByLabel('New password confirmation *').fill('654321')
  await page.getByRole('button', { name: 'Update password' }).click()

  // Logout
  await page.getByLabel('User account').click()
  await page.getByRole('menuitem', { name: 'Logout' }).click()

  // Login with the new password
  await page.getByLabel('Email *').click()
  await page.getByLabel('Email *').fill('user@mail.com')
  await page.getByLabel('Email *').press('Tab')
  await page.getByLabel('Password *').fill('654321')
  await page.getByRole('button', { name: 'Login' }).click()

  // Navigate to the Change password page
  await page.getByLabel('User account').click()
  await page.getByRole('menuitem', { name: 'Change password' }).click()

  // Fill in the change password form and submit
  await page.getByLabel('Current password *').click()
  await page.getByLabel('Current password *').fill('654321')
  await page.getByLabel('New password *').click()
  await page.getByLabel('New password *').fill('123456')
  await page.getByLabel('New password confirmation *').click()
  await page.getByLabel('New password confirmation *').fill('123456')
  await page.getByRole('button', { name: 'Update password' }).click()
})
