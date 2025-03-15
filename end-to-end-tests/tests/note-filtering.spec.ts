import { test, expect } from '@playwright/test'
import {
  login,
  addNewSection,
  getCurrentDateFormatted,
  getCurrentDay,
  getCurrentMonth,
  deleteNoteInList,
} from '../common/utils'
import { userCredentials, mainUrl } from '../common/config'

test.describe('Notes filtering', () => {
  test.beforeEach(async ({ page }) => {
    await login(page, userCredentials.email, userCredentials.password)
    await addNewSection(page, 'test', 'Test', '#000000')
    await page.goto(`${mainUrl}notes/section1`)
  })

  test('filter by book', async ({ page }) => {
    // Add new note
    await page.getByRole('button', { name: 'Add new note' }).click()

    // Add new book
    await page.getByRole('dialog').getByRole('button').filter({ hasText: /^$/ }).nth(2).click()
    await page.getByRole('textbox', { name: 'Search books' }).click()
    await page.getByRole('textbox', { name: 'Search books' }).fill('my book')
    await page.getByRole('option', { name: '+ Create new "my book" book' }).click()
    await page.getByRole('button', { name: "Update note's book" }).click()
    await expect(page.getByText('Note book is updated!')).toBeVisible()

    // Filter by book
    await page.getByText('my book').nth(1).click()
    await expect(page.locator('#root')).toContainText('my book')
    await page.getByRole('button', { name: 'Filter by book' }).click()
    await page.getByText('my book (1)').click()
    await page.getByRole('button', { name: 'Apply book filter' }).click()

    await deleteNoteInList(page)
  })

  test('filter by tags', async ({ page }) => {
    // Add new note
    await page.getByRole('button', { name: 'Add new note' }).click()

    // Add new tag
    await page.getByRole('dialog').getByRole('button').filter({ hasText: /^$/ }).nth(3).click()
    await page.getByRole('textbox', { name: 'Search tags' }).click()
    await page.getByRole('textbox', { name: 'Search tags' }).fill('tag1')
    await page.getByRole('option', { name: '+ Create new "tag1" tag' }).click()
    await page.getByRole('button', { name: "Update note's tags" }).click()
    await expect(page.getByText('Note tags are updated!')).toBeVisible()

    // Filter by tag
    await page.getByText('tag1').nth(1).click()
    await expect(page.locator('#root')).toContainText('tag1')
    await page.getByRole('button', { name: 'Filter by tags' }).click()
    await page.getByText('tag1 (1)').click()
    await page.getByRole('button', { name: 'Apply tags filter' }).click()

    await deleteNoteInList(page)
  })

  test('filter by date', async ({ page }) => {
    // Add new note
    await page.getByRole('button', { name: 'Add new note' }).click()
    await page.getByRole('dialog').getByRole('button').filter({ hasText: /^$/ }).nth(1).click()

    // Filter by date
    await page.getByRole('button', { name: 'Filter by date' }).click()
    await page.getByRole('button', { name: `${getCurrentDay()} ${getCurrentMonth()}` }).click()
    await expect(page.locator('#root')).toContainText(getCurrentDateFormatted())

    await deleteNoteInList(page)
  })

  test('in random order', async ({ page }) => {
    // Add new note
    await page.getByRole('button', { name: 'Add new note' }).click()
    await page.getByRole('dialog').getByRole('button').filter({ hasText: /^$/ }).nth(1).click()

    // Filter in random order
    await page.locator('label').filter({ hasText: 'In radom order' }).locator('span').nth(1).click()
    await expect(page.locator('#root')).toContainText('Found 1 note in random order')
    await page
      .locator('label')
      .filter({ hasText: 'In radom order' })
      .locator('span')
      .first()
      .click()

    await deleteNoteInList(page)
  })

  test('without book', async ({ page }) => {
    // Add new note
    await page.getByRole('button', { name: 'Add new note' }).click()
    await page.getByRole('dialog').getByRole('button').filter({ hasText: /^$/ }).nth(1).click()

    // Filter in random order
    await page.locator('label').filter({ hasText: 'Without book' }).locator('span').nth(1).click()
    await expect(page.locator('#root')).toContainText('Found 1 note without book')
    await page.locator('label').filter({ hasText: 'Without book' }).locator('span').first().click()

    await deleteNoteInList(page)
  })

  test('without tags', async ({ page }) => {
    // Add new note
    await page.getByRole('button', { name: 'Add new note' }).click()
    await page.getByRole('dialog').getByRole('button').filter({ hasText: /^$/ }).nth(1).click()

    // Filter in random order
    await page.locator('label').filter({ hasText: 'Without tags' }).locator('span').nth(1).click()
    await expect(page.locator('#root')).toContainText('Found 1 note without tags')
    await page.locator('label').filter({ hasText: 'Without tags' }).locator('span').first().click()

    await deleteNoteInList(page)
  })

  test('full-text search', async ({ page }) => {
    // Add new note
    await page.getByRole('button', { name: 'Add new note' }).click()
    await page.getByRole('textbox', { name: 'Title' }).click()
    await page.getByRole('textbox', { name: 'Title' }).fill('Test')
    await page.getByRole('paragraph').click()
    await page.getByRole('paragraph').fill('Test content')
    await page.getByRole('button', { name: 'Save' }).click()
    await expect(page.getByText('Note is updated!')).toBeVisible()
    await page.getByRole('dialog').getByRole('button').filter({ hasText: /^$/ }).nth(1).click()

    // Search
    await page.getByRole('textbox', { name: 'Search' }).click()
    await page.getByRole('textbox', { name: 'Search' }).fill('test')
    await page.getByRole('textbox', { name: 'Search' }).press('Enter')
    await expect(page.locator('#root')).toContainText('test')
    await expect(page.locator('#root')).toContainText('content...')

    await deleteNoteInList(page)
  })

  test('autocomplete filtering', async ({ page }) => {
    // Add new note
    await page.getByRole('button', { name: 'Add new note' }).click()

    // Add new book
    await page.getByRole('dialog').getByRole('button').filter({ hasText: /^$/ }).nth(2).click()
    await page.getByRole('textbox', { name: 'Search books' }).click()
    await page.getByRole('textbox', { name: 'Search books' }).fill('my book')
    await page.getByRole('option', { name: '+ Create new "my book" book' }).click()
    await page.getByRole('button', { name: "Update note's book" }).click()
    await expect(page.getByText('Note book is updated!')).toBeVisible()

    // Add new tag
    await page.getByRole('dialog').getByRole('button').filter({ hasText: /^$/ }).nth(3).click()
    await page.getByRole('textbox', { name: 'Search tags' }).click()
    await page.getByRole('textbox', { name: 'Search tags' }).fill('tag1')
    await page.getByRole('option', { name: '+ Create new "tag1" tag' }).click()
    await page.getByRole('button', { name: "Update note's tags" }).click()
    await expect(page.getByText('Note tags are updated!')).toBeVisible()

    await page.getByRole('dialog').getByRole('button').filter({ hasText: /^$/ }).nth(1).click()

    // Filter by book
    await page.getByRole('textbox', { name: 'Search' }).click()
    await page.getByRole('textbox', { name: 'Search' }).fill('my book')
    await page.getByRole('option', { name: 'my book' }).click()

    // Filter by tag
    await page.getByRole('textbox', { name: 'Search' }).click()
    await page.getByRole('textbox', { name: 'Search' }).fill('tag1')
    await page.getByRole('option', { name: 'tag1' }).click()
    await expect(page.locator('#root')).toContainText('tag1')

    await deleteNoteInList(page)
  })
})
